using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LSystems {
public partial class PLSystem : IEnumerable<PLSystem.Word[]>
{

    [System.Serializable]
    public class Word
    {
        public string name;
        public int count;
        public float param;
        private bool hasCount = false,
          hasParam = false;
        public float angle {
          get { return param; }
        }
        public float distance {
          get { return param; }
        }
        public virtual string prettyName {
          get;
          private set;
        }
        public Word(string name) {
            this.name = name;
            prettyName = name;
        }
        public Word(string name, int count)
        : this(name) {
            this.count = count;
            hasCount = true;
            prettyName = name+"("+count+")";
        }
        public Word(string name, float param)
        : this(name) {
            this.param = param;
            hasParam = true;
            prettyName = name+"("+param+")";
        }
        public Word(string name, int count, float param)
        : this(name) {
            this.param = param;
            hasParam = true;
            this.count = count;
            hasCount = true;
            prettyName = name+"("+count+", "+param+")";
        }
        public static Word IniitalizerF(string name, params object[] args) {
          return new Word(name, (float)args[0]);
        }
        public virtual Word Copy() {
          if(hasCount) {
            if(hasParam) {
              return new Word( name, count, param );
            } else {
              return new Word( name, count );
            }
          } else {
            if(hasParam) {
              return new Word(name, param);
            } else {
              return new Word(name);
            }
          }
        }
        /** Return a terminal version of this word, meant for closing sequences and branches */
        public virtual Word Terminal() {
          return new Word(this.name);
        }
        public bool isTerminator
        {
            get
            {
                return string.IsNullOrEmpty(name);
            }
        }
        public static Word Terminator {
          get {
            return new Word(null);
          }
        }
        public static implicit operator string(Word word) {
          return word.name;
        }
        public override string ToString() {
          return prettyName;
        }
    }

    public delegate Word WordScheme(Word word);

    public static Word IdentityScheme(Word word) {
        return word;
    }

    public static Word NullScheme(Word word) {
        return null;
    }

    public static WordScheme SetCountScheme(int count) {
        return word =>
        {
            word.count = count;
            return word;
        };
    }

    public static Word IncrementCountScheme(Word word) {
        word.count++;
        return word;
    }

    public static Word DecrementCountScheme(Word word) {
        word.count--;
        return word;
    }

    public static WordScheme ConstParamScheme(float value) {
        return word =>
        {
            word.param = value;
            return word;
        };
    }

    public static WordScheme ConstScheme(Word constant) {
        return word => constant.Copy();
    }

    public static WordScheme ParamFuncScheme(Func<float, float> func) {
        return word =>
        {
            word.param = func(word.param);
            return word;
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
        public int GetSuccessor(Word pred, IList<Word> dest, int start) {
            for(int i=0;i<successor.Count;i++) {
                dest[start + i] = successor[i](pred.Copy());
            }
            return successor.Count;
        }
    }

    public Word[] axiom;
    public Dictionary<string, List<Production>> productions
        = new Dictionary<string, List<Production>>();

    public void SetAxiom(params Word[] axiom) {
        this.axiom = new Word[axiom.Length+1];
        System.Array.Copy(axiom, this.axiom, axiom.Length);
        this.axiom[axiom.Length] = Word.Terminator;
    }

    public void AddProduction(string name, params WordScheme[] successor) {
        List<Production> prods = null;
        if(!productions.TryGetValue(name, out prods)) {
            prods = new List<Production>();
            productions[name] = prods;
        }
        var newProd = new Production();
        newProd.successor = successor;
        prods.Add(newProd);
    }

    public void Derive(IList<Word> predecessor, IList<Word> successor)
    {
        List<Production> prods = null;
        var len = predecessor.Count;
        var index = 0;
        bool found = false;
        for (int i = 0; !predecessor[i].isTerminator; i++)
        {
            prods = null;
            found = false;
            if (productions.TryGetValue(predecessor[i].name, out prods))
            {
                var last = i > 0 ? predecessor[i - 1] : null;
                var next = i < (len - 1) ? predecessor[i + 1] : null;
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
                successor[index] = predecessor[i].Copy();
                index++;
            }
        }
        // null terminator
        successor[index] = PLSystem.Word.Terminator;
    }

    public static string DerivationToString(IList<Word> derivation) {
      return string.Join(
            string.Empty,
            derivation.TakeWhile(w => !w.isTerminator).Select(d => d.prettyName).ToArray());
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
                int i=0;
                for(;!system.axiom[i].isTerminator;i++) {
                  current[i] = system.axiom[i];
                }
                current[i] = system.axiom[i];
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
}
