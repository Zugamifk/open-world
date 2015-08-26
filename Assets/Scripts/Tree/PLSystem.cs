using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PLSystem : IEnumerable<PLSystem.Word[]>
{

    [System.Serializable]
    public struct Word
    {
        public string name;
        public string prettyName;
        public int count;
        public float param;
        public Word(string name) {
            this.name = name;
            prettyName = name;
            count = 0;
            param = 0;
        }
        public Word(string name, int count)
        : this(name) {
            this.prettyName = string.Format("{0}({1})", name, count);
            this.count = count;
        }
        public Word(string name, float param)
        : this(name) {
            this.prettyName = string.Format("{0}({1})", name, param);
            this.param = param;
        }
        public Word(string name, int count, float param)
        : this(name) {
            this.prettyName = string.Format("{0}({1},{2})", name, count, param);
            this.param = param;
            this.count = count;
        }
        public bool isTerminator
        {
            get
            {
                return string.IsNullOrEmpty(name);
            }
        }
    }

    public delegate void WordScheme(Word pred, ref Word target);

    public static void IdentityScheme(Word pred, ref Word target) {
        target = pred;
    }

    public static void NullScheme(Word pred, ref Word target) {
        target = default(Word);
    }

    public static WordScheme SetCountScheme(int count) {
        return (Word w0, ref Word w1) =>
        {
            w1 = w0;
            w1.count = count;
        };
    }

    public static void IncrementCountScheme(Word pred, ref Word target) {
        target = pred;
        target.count++;
    }

    public static void DecrementCountScheme(Word pred, ref Word target) {
        target = pred;
        target.count--;
    }

    public static WordScheme ConstParamScheme(float value) {
        return (Word w0, ref Word w1) =>
        {
            w1 = w0;
            w1.param = value;
        };
    }

    public static WordScheme ConstScheme(Word word) {
        return (Word w0, ref Word w1) => w1 = word;
    }

    public static WordScheme ParamFuncScheme(Func<float, float> func) {
        return (Word w0, ref Word w1) =>
        {
            w1 = w0;
            w1.param = func(w0.param);
        };
    }

    public class Production
    {
        public string left;
        public string right;
        public Predicate<Word> condition;
        public IList<WordScheme> successor;
        public static Predicate<Word> Always = w => true;
        public Production() {
            left = string.Empty;
            right = string.Empty;
            condition = Always;
            successor = new WordScheme[] { PLSystem.IdentityScheme };
        }
        public bool CheckContext(Word last, Word next) {
            return (string.IsNullOrEmpty(left) || left.Equals(last.name)) &&
                (string.IsNullOrEmpty(right) || right.Equals(next.name));
        }
        public int GetSuccessor(Word pred, Word[] dest, int start) {
            for(int i=0;i<successor.Count;i++) {
                successor[i](pred, ref dest[start + i]);
            }
            return successor.Count;
        }
    }

    public Word axiom;
    public Dictionary<string, List<Production>> productions
        = new Dictionary<string, List<Production>>();

    public void AddProduction(string name, IList<WordScheme> succcessor) {
        List<Production> prods = null;
        if(!productions.TryGetValue(name, out prods)) {
            prods = new List<Production>();
            productions[name] = prods;
        }
        var newProd = new Production();
        newProd.successor = succcessor;
        prods.Add(newProd);
    }

    public void Derive(Word[] predecessor, Word[] successor)
    {
        List<Production> prods = null;
        var len = predecessor.Length;
        var index = 0;
        bool found = false;
        for (int i = 0; !predecessor[i].isTerminator; i++)
        {
            prods = null;
            found = false;
            if (productions.TryGetValue(predecessor[i].name, out prods))
            {
                var last = i > 0 ? predecessor[i - 1] : default(Word);
                var next = i < (len - 1) ? predecessor[i + 1] : default(Word);
                foreach (var p in prods)
                {
                    if (p.CheckContext(last, next))
                    {
                        index += p.GetSuccessor(predecessor[i], successor, index);
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                successor[index] = predecessor[i];
                index++;
            }
        }
        // null terminator
        predecessor[index] = default(Word);
    }

    #region IEnumerable
    public class Enumerator : IEnumerator<Word[]>
    {
        private PLSystem system;
        private Word[] current;
        private Word[] oldCurrent;
        private bool initialized;
        private const int MAX_LENGTH = 2048;
        public Enumerator(PLSystem system)
        {
            this.system = system;
            current = new Word[MAX_LENGTH];
            oldCurrent = new Word[MAX_LENGTH]; ;
            initialized = false;
        }
        public Word[] Current
        {
            get { return current; }
        }
        object IEnumerator.Current
        {
            get { return (object)current; }
        }
        public bool MoveNext()
        {
            if (initialized)
            {
                var temp = oldCurrent;
                oldCurrent = current;
                current = temp;
                system.Derive(oldCurrent, current);
            }
            else
            {
                current[0] = system.axiom;
                current[1] = default(PLSystem.Word);
                initialized = true;
            }
            return true;
        }
        public void Reset()
        {
            initialized = false;
        }
        public void Dispose() { }

    }
    public IEnumerator<Word[]> GetEnumerator()
    {
        return new Enumerator(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    #endregion
}
