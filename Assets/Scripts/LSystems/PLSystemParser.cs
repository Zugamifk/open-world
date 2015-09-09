using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LSystems {
  public partial class PLSystem : IEnumerable<PLSystem.Word[]> {
    public delegate Word WordInitializerCallback(string name, params object[] parameterValues);
    public class Parser : IUnitTestable  {
      private Dictionary<string, Type[]> signatures;
      private Dictionary<string, WordInitializerCallback> initializers;
      private Regex wordMatch, namesMatch, paramsMatch;
      private Regex productionMatch, predecessorMatch, successorMatch;

      //TODO: allow words longer than one character in length
      private const string defaultWordNamesMatchString = @"[\+\-&\^<>\[\]\|]";
      private const string namesMatchString = @"^\s*[^\(]+";
      private const string paramsMatchString = @"(?<=[\(,]\s*)[0-9\.f]+";
      private readonly string wordMatchString = "";
      private readonly string productionMatchString = "";

      public Parser() {
        signatures = new Dictionary<string, Type[]>();
        initializers = new Dictionary<string, WordInitializerCallback>();

        AddSignature("+", typeof(float));
        AddInitializer("+", Word.IniitalizerF);
        AddSignature("-", typeof(float));
        AddInitializer("-", Word.IniitalizerF);
        AddSignature("^", typeof(float));
        AddInitializer("^", Word.IniitalizerF);
        AddSignature("&", typeof(float));
        AddInitializer("&", Word.IniitalizerF);
        AddSignature("<", typeof(float));
        AddInitializer("<", Word.IniitalizerF);
        AddSignature(">", typeof(float));
        AddInitializer(">", Word.IniitalizerF);

        wordMatchString = string.Format(@"(?:[A-Z]|{0})(?:\((?:[^\)]*)?\))?", defaultWordNamesMatchString);
        productionMatchString = string.Format(@"{0}\s*\-\->\s*({1})*", wordMatchString, wordMatchString);

        wordMatch = new Regex(wordMatchString);
        namesMatch = new Regex(namesMatchString);
        paramsMatch = new Regex(paramsMatchString);
      }

      public void AddSignature(string name, params Type[] signature) {
        signatures[name] = signature;
      }

      public void AddInitializer(string name, WordInitializerCallback initializer) {
        initializers[name] = initializer;
      }

      public Word ParseWord(string input) {
        string name = namesMatch.Match(input).Value;
        MatchCollection paramValues = paramsMatch.Matches(input);
        Type[] signature = null;
        if(signatures.TryGetValue(name, out signature)) {
          if(signature.Length == paramValues.Count) {
            int ti = 0;
            object[] parameters = new object[paramValues.Count];
            foreach(Match match in paramValues) {
              if(signature[ti].Equals(typeof(float))) {
                float val;
                if(!float.TryParse(match.Value, out val)) {
                  Debug.LogErrorFormat("Word \"{0}\" expected a float value at argument {1}, got \"{2}\" instead.", name, ti, match.Value);
                } else {
                  parameters[ti] = val;
                }
              } else
              if(signature[ti].Equals(typeof(int))) {
                int val;
                if(!int.TryParse(match.Value, out val)) {
                  Debug.LogErrorFormat("Word \"{0}\" expected an int value at argument {1}, got \"{2}\" instead.", name, ti, match.Value);
                } else {
                  parameters[ti] = val;
                }
              } else {
                Debug.LogErrorFormat("PLSystemParser can not parse arguments of type {0}!", signature[ti]);
              }
              ti++;
            }
            Word result = null;
            WordInitializerCallback initializer = null;
            if(initializers.TryGetValue(name, out initializer)) {
              result = initializer(name, parameters);
            } else {
              result = new Word(name);
            }
            return result;
          } else {
            Debug.LogErrorFormat("String \"{0}\" does not contain the correct number of parameters! \"{1}\" expects {2} parameters", input, name, signature.Length);
          }
        } else {
          return new Word(name);
        }

        return null;
      }
      public Word[] ParseString(string input) {
        var words = wordMatch.Matches(input);
        Word[] result = new Word[words.Count];
        int i=0;
        foreach(Match word in words) {
          result[i] = ParseWord(word.Value);
          i++;
        }
        return result;
      }

      public WordScheme ParseWordScheme(string input) {
        string name = namesMatch.Match(input).Value;
        MatchCollection paramValues = paramsMatch.Matches(input);
        Type[] signature = null;
        if(signatures.TryGetValue(name, out signature)) {
          if(signature.Length == paramValues.Count) {
            int ti = 0;
            string[] parameters = new string[paramValues.Count];
            foreach(Match match in paramValues) {
              parameters[ti] = match.Value;
              ti++;
            }

            WordScheme result = null;
            // GET SCHEME
            return result;
          } else {
            Debug.LogErrorFormat("String \"{0}\" does not contain the correct number of parameters! \"{1}\" expects {2} parameters", input, name, signature.Length);
          }
        } else {
          return null;
        }

        return null;
      }
      public string ParseProduction(string input, out IList<WordScheme> successor) {
        var name = namesMatch.Match(input).Value;
        successor = null;
        return name;
      }

      public void Test() {
        string input = "+(45)<(15)F|[&(30)]";
        Word[] derivation = ParseString(input);
        string output = DerivationToString(derivation);
        Debug.Log(input+" --> ["+derivation.Length+"]"+output);
      }
    }
  }
}
