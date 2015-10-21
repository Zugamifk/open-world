/*
Monad based parser
Thanks to:
 	http://www.cs.nott.ac.uk/~gmh/monparsing.pdf
	http://blogs.msdn.com/b/wesdyer/archive/2008/01/11/the-marvels-of-monads.aspx
	http://mikehadlow.blogspot.ca/2011/01/monads-in-c-3-creating-our-first-monad.html
*/
﻿using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Lambdas;
using Extensions;

namespace Grammars {

	public struct ParserState {
		public string input;
		public int position;
		public ParserState(string input) {
			this.input = input;
			position = 0;
			terminated = string.IsNullOrEmpty(input);
		}
		public ParserState(string input, int position) {
			this.input = input;
			this.position = position;
			terminated = position >= input.Length;
		}
		private bool terminated;
		public bool Terminated {
			get {
				return terminated;
			}
		}
		public ParserState Move(int length) {
			return new ParserState(input, position+length);
		}
		public static implicit operator ParserState(string value) {
			return new ParserState(value);
		}
	}

	public struct Token<T> {
		public T value;
		public ParserState state;
		public string valueString;
		public Token(T value, ParserState state) {
			this.value = value;
			this.state = state;
			valueString = string.Empty;
		}
		string ValueString {
			get {
				if(string.IsNullOrEmpty(valueString)) {
					int maxCount = 3;
					if(value is IEnumerable<int>) {
						var sb = new StringBuilder();
						sb.Append("(");
						int i=0;
						foreach(var val in value as IEnumerable<int>) {
							if(i>0) sb.Append(", ");
							if(++i>maxCount) {
								sb.Append("...");
								break;
							}
							sb.Append(val);
						}
						sb.Append(")");
						valueString = sb.ToString();
					} else
					if(value is IEnumerable<string>) {
						var sb = new StringBuilder();
						sb.Append("(");
						int i=0;
						foreach(var val in value as IEnumerable<string>) {
							if(i>0) sb.Append(", ");
							if(++i>maxCount) {
								sb.Append("...");
								break;
							}
							sb.Append(val);
						}
						sb.Append(")");
						valueString = sb.ToString();
					} else
					{
						valueString = value.ToString();
					}
				}
				return valueString;
			}
		}
		public override string ToString() {
			return string.Format("{0} {1} : \"{2}\"", typeof(T), ValueString, state.input.Substring(state.position));
		}
	}

	public delegate IEnumerable<Token<T>> Parser<T>(ParserState state);

	public static class Parsers {

		/** Consumes nothing, returns a constant value */
		public static Parser<T> Result<T>(this T result) {
			return state => (new Token<T>(result, state)).Single();
		}

		public static Parser<T> Zero<T>() {
			 return state => Enumerable.Empty<Token<T>>();
		}

		public static Parser<char> Item = state =>
			state.Terminated ? Enumerable.Empty<Token<char>>() :
			(new Token<char>(state.input[state.position], state.Move(1))).Single();

		public static Parser<T> Identity<T>() {
			return Result(default(T));
		}

		public static Parser<T> Bind<S,T>(this Parser<S> first, Func<S, Parser<T>> func) {
			return state => first(state).SelectMany(token => func(token.value)(token.state));
		}

		public static Parser<char> Sat(this Predicate<char> sat) {
			return Item.Bind<char, char>(c => sat(c) ? Result(c) : Zero<char>() );
		}

		public static Parser<char> Char(this char c) {
			return Sat(x=>x==c);
		}

		public static Parser<char> Digit = Sat(c=>c>='0'&&c<='9');
		public static Parser<char> Lower = Sat(c=>c>='a'&&c<='z');
		public static Parser<char> Upper = Sat(c=>c>='A'&&c<='Z');
		public static Parser<char> Space = Sat(c=>c==' '||c=='\n'||c=='\t');

		public static Parser<T> Plus<T>(this Parser<T> self, params Parser<T>[] others) {
			return state => self(state).Concat(others.SelectMany(fun=>fun(state)));
		}

		public static Parser<char> Letter = Lower.Plus(Upper);
		public static Parser<char> Alphanum = Letter.Plus(Digit);

		public static Parser<string> Word =
			Letter.Bind<char,string>(x=>Word.Bind<string, string>(xs=>Result(x+xs)))
			.Plus(Result(""));

		public static Parser<string> Many(this Parser<char> self) {
			return self.Bind<char, string>(
				t=>self.Many().Bind<string, string>(
					ts=>Result(t+ts)
					)
				).Plus(Result(""));
		}

		public static Parser<IEnumerable<T>> Many<T>(this Parser<T> self) {
			return self.Bind<T, IEnumerable<T>>(
				t=>self.Many().Bind<IEnumerable<T>,IEnumerable<T>>(
					ts=>Result(t.Cons(ts))
					)
				).Plus(Result(Enumerable.Empty<T>()));
		}

		public static Parser<string> Ident = Lower.Bind<char, string>(c=>Alphanum.Many().Bind<string, string>(cs=>Result(c+cs)));
		public static Parser<string> Spaces = Many(Space);

		public static Parser<string> Many1(this Parser<char> self) {
			return self.Bind<char, string>(
				t=>self.Many().Bind<string, string>(
					ts=>Result(t+ts)
					)
				);
		}

		public static Parser<int> Nat = Digit.Bind<char, int>(n=>Result((int)(n-'0'))).ChainL1(Result<Func<int,int,int>>((n,m)=>n*10+m));
		public static Parser<int> Int =
			Char('-').Bind<char, Func<int,int>>(c=>Result<Func<int,int>>(n=>-n))
			.Plus(Result<Func<int,int>>(Lambda.Identity<int>))
			.Bind<Func<int,int>, int>(f=>Nat.Bind<int,int>(n=>Result(f(n))));
		public static Parser<float> Flt = Int.Bind<int, float>(whole =>
			Char('.').Skip(
				Digit.Bind<char, float>(n=>Result((float)(n-'0')*0.1f))
				.ChainL1(Result<Func<float,float,float>>((n,m)=>n*0.1f+m))
				.Bind<float, float>(frac => Result((float)whole+frac))
			).Plus(Result((float)whole))
		);

		public static Parser<T> Skip<S,T>(this Parser<S> skip, Parser<T> next) {
			return skip.Bind<S,T>(_=>next);
		}

		public static Parser<IEnumerable<int>> Ints =
			Char('[').Bind<char,IEnumerable<int>>( _0=>
				Int.Bind<int, IEnumerable<int>>( n=>
					Many(
						Char(',').Bind<char, int>(_comma=>Int)
					).Bind<IEnumerable<int>, IEnumerable<int>>( ns =>
						Char(']').Bind<char, IEnumerable<int>>( _1 =>
							Result(n.Cons(ns))
						)
					)
				)
			);

			public static Parser<IEnumerable<T>> SeparateBy<S,T>(this Parser<T> match, Parser<S> sep) {
				return match.Bind<T, IEnumerable<T>>( x =>
							Many(sep.Skip(match)
						).Bind<IEnumerable<T>, IEnumerable<T>>( xs =>
							Result(x.Cons(xs))
						)
					);
			}

			public static Parser<T> Bracket<S,T,U>(this Parser<T> inner, Parser<S> open, Parser<U> close) {
				return open.Skip(inner).Bind<T,T>(result=>close.Skip(Result(result)));
			}

			public static Parser<IEnumerable<int>> Ints2 =
				Int.SeparateBy(Char(',')).Bracket(Char('['), Char(']'));

			public static void LogParserOutput<T>(IEnumerable<Token<T>> output, int maxValues = 10) {
				int i=1;
				foreach(var token in output) {
					if(i++>maxValues) break;
					Debug.Log(token);
				}
				if(i==1) Debug.Log("Nothing parsed!");
			}

			// INT EXPRESSIONS
			// as an exercise
			public static class IntArithmetic {

				public static Parser<int> Parser {
					get {
						return Expression;
					}
				}

				public static Parser<Func<int,int,int>> AddOperation =
					Char('+').Operator<char, int>((a,b)=>a+b)
						.Plus(
					Char('-').Operator<char, int>((a,b)=>a-b)
						);

				public static Parser<Func<int,int,int>> MulOperation =
					Char('*').Operator<char, int>((a,b)=>a*b)
						.Plus(
					Char('/').Operator<char, int>((a,b)=>a/b)
				);

				public static Parser<Func<int,int,int>> ExpOperation =
					Char('^').Operator<char, int>(Extensions.Math.IntPow);

				private static IEnumerable<Token<int>> Factor(ParserState state) {
					return Nat.Plus(new Parser<int>(Expression).Bracket(Char('('), Char(')')))(state);
				}

				private static IEnumerable<Token<int>> Terminal(ParserState state) {
					return new Parser<int>(Factor).ChainR1(ExpOperation).ChainL1(MulOperation)(state);
				}

				private static IEnumerable<Token<int>> Expression(ParserState state) {
					return new Parser<int>(Terminal).ChainL1(AddOperation)(state);
				}
			}

			public static Parser<T> ChainL1<T>(this Parser<T> self, Parser<Func<T,T,T>> operation) {
				return self.Bind<T,T>( x0 =>
					Many(
						operation.Bind<Func<T,T,T>, Func<T,T>>( f =>
							self.Bind<T, Func<T,T>>( y =>
								Result<Func<T,T>>(x => f(x,y))
							)
						)
					).Bind<IEnumerable<Func<T,T>>, T>( fys =>
						Result(fys.Aggregate(x0, (xi, fy) => fy(xi)))
					)
				);
			}

			public static Parser<Func<T,U,V>> Operator<S,T,U,V>(this Parser<S> self, Func<T,U,V> op) {
				return self.Bind<S,Func<T,U,V>>(_=>Result(op));
			}
			public static Parser<Func<T,T,T>> Operator<S,T>(this Parser<S> self, Func<T,T,T> op) {
				return self.Bind<S,Func<T,T,T>>(_=>Result(op));
			}

			public static Parser<T> ChainR1<T>(this Parser<T> self, Parser<Func<T,T,T>> operation) {
				return self.Bind<T,T>( x =>
					operation.Bind<Func<T,T,T>,T>( op =>
						self.ChainR1(operation).Bind<T,T>( y =>
							Result(op(x,y))
						)
					).Plus(Result(x))
				);
			}

			public static Parser<T> ChainL<T>(this Parser<T> self, Parser<Func<T,T,T>> op, T value) {
				return self.ChainL1(op).Plus(Result(value));
			}

			public static Parser<T> ChainR<T>(this Parser<T> self, Parser<Func<T,T,T>> op, T value) {
				return self.ChainR1(op).Plus(Result(value));
			}

			// possibly inefficient!
			public static Parser<T> First<T>(this Parser<T> p) {
				return state => {
					var result = p(state);
					if(result.Any()) {
						return result.Take(1);
					} else {
						return result;
					}
				};
			}

			public static Parser<T> PlusDet<T>(this Parser<T> a, params Parser<T>[] others) {
				return a.Plus(others).First();
			}

			public static Parser<string> Junk = Spaces;

			public static Parser<T> Parse<T>(this Parser<T> self) {
				return Junk.Skip(self).First();
			}

			public static Parser<T> Tokenize<T>(this Parser<T> self) {
				return self.Bind<T,T>(v =>
					Junk.Skip(Result(v))
				).First();
			}

			public static Parser<int> Natural = Nat.Tokenize();
			public static Parser<int> Integer = Int.Tokenize();
			public static Parser<float> Float = Flt.Tokenize();

			public static Parser<string> String(string str) {
				return Word.Bind<string, string>(w=>str == w ? Result(str) : Zero<string>());
			}

			public static Parser<string> Symbol(string symbol) {
				return String(symbol).First().Tokenize();
			}

			public static Parser<T> Cond<T>(this Parser<T> parser, Predicate<T> condition) {
				return parser.Bind<T,T>(t => condition(t) ? Result(t) : Zero<T>());
			}

			public static Parser<string> Identifier(params string[] keywords) {
				return Ident.First().Cond<string>(w=>!keywords.Contains(w)).Tokenize();
			}

			public static Parser<IEnumerable<T>> With<S,T,U>(Func<S, Parser<T>> selector, Parser<U> separator, params S[] values) {
				var result = selector(values[0]).Bind<T, IEnumerable<T>>(v=>Result(v.Single()));
				for(int i=1;i<values.Length;i++) {
					result = result.Bind<IEnumerable<T>, IEnumerable<T>>(xs=>
						separator.Skip(
							selector(values[i]).Bind<T, IEnumerable<T>>( x =>
								Result(x.Cons(xs))
							)
						)
					);
				}
				return result;
			}

			public class LambdaExpression<T> {
				public Parser<Func<T,T>> Parser {
					get {
						return Expression;
					}
				}

				public Parser<Func<T,T>> Variable {
					get {
						return Result<Func<T,T>>(Lambda.Identity<T>);
					}
				}

				public Parser<Func<T,T>> Expression {
					get {
						return Variable;
					}
				}
			}

			public class ParserTest : IUnitTestable {
				public string input = "";
				public void Test() {
					// var result = Result(1);
					// var zero = Zero<object>();
					// var item = Item;					var identity = Identity<int>();
					// var sat = Sat(c=>"poo".Contains(c));
					// var ch = Char('a');
					//
					// var compareInts = Ints.Bind<IEnumerable<int>, IEnumerable<int>>(
					// 	i0 => Spaces.Bind<string, IEnumerable<int>> (
					// 		_ => Ints2.Bind<IEnumerable<int>, IEnumerable<int>>(
					// 			i1 => Result(i0.Concat(i1))
					// 	)));

					var test = Float;
					LogParserOutput(test(input));
				}
			}
	}
}
