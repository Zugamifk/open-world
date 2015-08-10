using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LSystem : IEnumerable<string>, IUnitTestable{
	public string axiom;
	public string[] productions;

	public LSystem(){
		productions = new string[256];
	}
	public LSystem(string axiom, params string[] productions){
		this.axiom = axiom;
		this.productions = new string[256];
		for(int i=0;i<productions.Length;i+=2) {
			this.productions[(int)productions[i][0]] = productions[i+1];
		}
	}

	public void AddProduction(char c, string succ) {
		productions[(int)c] = succ;
	}

	public string Successor(char c) {
		var prod = productions[(int)c];
		if(!string.IsNullOrEmpty(prod)) return prod;
		return c.ToString();
	}

	public string Derive(string a) {
		return System.String.Join("",a.Select(c=>Successor(c)).ToArray());
	}

#region IEnumerable
	public class Enumerator : IEnumerator<string> {
		private LSystem system;
		private string current;
		private bool initialized;
		public Enumerator(LSystem system) {
			this.system = system;
			current = "";
			initialized = false;
		}
		public string Current {
			get { return current; }
		}
		object IEnumerator.Current {
			get { return (object)current; }
		}
		public bool MoveNext() {
			if(initialized) {
				current = system.Derive(current);
			} else {
				current = system.axiom;
				initialized = true;
			}
			return true;
		}
		public void	 Reset() {
			initialized = false;
		}
		public void Dispose(){}

	}
	public IEnumerator<string> GetEnumerator() {
		return new Enumerator(this);
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
#endregion
#region IUnitTestable
	public void Test() {
		var system = new LSystem("b",
		 	"a", "ab",
			"b", "a");
		system.Take(10).ForEach(Debug.Log);
	}
#endregion
}
