using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Grammars;

namespace LSystems {
  public partial class PLSystem : IEnumerable<PLSystem.Word[]> {
    public delegate Word WordInitializerCallback(string name, params object[] parameterValues);
    public class Parser : IUnitTestable  {
      private Dictionary<string, Type[]> signatures;
      private Dictionary<string, WordInitializerCallback> initializers;

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
      }

      public void AddSignature(string name, params Type[] signature) {
        signatures[name] = signature;
      }

      public void AddInitializer(string name, WordInitializerCallback initializer) {
        initializers[name] = initializer;
      }


      public Parser<string> IdentifierP {
        get {
          return Parsers.Letter.PlusDet(
            Parsers.Char('+'), Parsers.Char('-'),
            Parsers.Char('^'), Parsers.Char('&'),
            Parsers.Char('<'), Parsers.Char('>'),
            Parsers.Char('|'), Parsers.Char('['), Parsers.Char(']')
          ).Tokenize().Bind<char, string>(c=>Parsers.Result(c.ToString()));
        }
      }

      public Parser<Word> WordP {
        get {
          return IdentifierP
            .Bind<string, Word>(name => {
              Type[] signature = null;
              if(signatures.TryGetValue(name, out signature)) {
                return Parsers.With<Type, object, char>(
                  t => {
                    if(typeof(float).Equals(t)) {
                      return Parsers.Float.Bind<float, object>(f=>Parsers.Result((object)f));
                    } else
                    if(typeof(int).Equals(t)) {
                      return Parsers.Integer.Bind<int, object>(i=>Parsers.Result((object)i));
                    } else {
                      return Parsers.Zero<object>();
                    }
                  },
                  Parsers.Char(','),
                  signature
                ).Bracket(Parsers.Char('('), Parsers.Char(')'))
                .Bind<IEnumerable<object>, Word>(
                  args => {
                    WordInitializerCallback initializer = null;
                    if(initializers.TryGetValue(name, out initializer)) {
                      return Parsers.Result(initializer(name, args.ToArray()));
                    } else {
                      return Parsers.Result(new Word(name));
                    }
                  }
                );
              } else {
                return Parsers.Result(new Word(name));
              }
          }
          );
      }
    }

    public Parser<IEnumerable<Word>> StringP {
      get {
        return WordP.Tokenize().Many();
      }
    }
    //
    // public Parser<WordScheme> WordSchemeP {
    //   get {
    //     return IdentifierP.Bind<string, WordScheme>( name =>
    //       Type[] signature = null;
    //       if(signatures.TryGetValue(name, out signature)) {
    //         return Parsers.With<Type, object, char>(
    //           t => {
    //             if(typeof(float).Equals(t)) {
    //               return Parsers.Float.Bind<float, object>(f=>Parsers.Result((object)f));
    //             } else
    //             if(typeof(int).Equals(t)) {
    //               return Parsers.Integer.Bind<int, object>(i=>Parsers.Result((object)i));
    //             } else {
    //               return Parsers.Zero<object>();
    //             }
    //           },
    //           Parsers.Char(','),
    //           signature
    //         ).Bracket(Parsers.Char('('), Parsers.Char(')'))
    //         .Bind<IEnumerable<object>, Word>(
    //           args => {
    //             WordInitializerCallback initializer = null;
    //             if(initializers.TryGetValue(name, out initializer)) {
    //               return Parsers.Result(initializer(name, args.ToArray()));
    //             } else {
    //               return Parsers.Result(new Word(name));
    //             }
    //           }
    //         );
    //       } else {
    //         return Parsers.Result(new Word(name));
    //       }
    //     );
    //   }
    // }

    public Parser<Production> ProductionP {
      get {
        return IdentifierP.Bind<string, Production>( name =>
          Parsers.String("=>").Tokenize().Skip<string, Production>(
            Parsers.Result<Production>(null)
          )
        );
      }
    }

    public string testinput = "]+(3)";

      public void Test() {
        Word[] derivation = StringP(testinput).FirstOrDefault().value.ToArray();
        string output = DerivationToString(derivation);
        Debug.Log(testinput+" --> ["+derivation.Length+"] "+output);
      }
    }
  }
}
