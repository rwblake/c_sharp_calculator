using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace calculator {

	class Token {

        public string name;
        public decimal? val;

		public Token(string name, decimal? val = null) {
			this.name = name;
            this.val = val;
		}

        public string Repr() {
            if (this.val == null) {  // has no value
                return $"{this.name}";
            } else {
                return $"{this.name}:{this.val}";
            }
        }
	}

    class TknData {

        public string name;
        public string regex;
        public bool hasValue;

        public TknData(string name, string regex, bool hasValue=false) {
            this.name = name;
            this.regex = regex;
            this.hasValue = hasValue;
        }
    }

    class LexerResult {

        public List<Token> tokens;
        public Error? error;

        public LexerResult(List<Token> tokens, Error? error=null) {
            this.tokens = tokens;
            this.error = error;
        }
    }

    class Lexer {

        List<TknData> tokData = new List<TknData>(){
            // new TknData("FLOAT", @"(\d*\.\d+)|(\d+\.\d*)", true),
            new TknData("INT", @"\d+", true),
            // new TknData("LOG", @"log"),
            new TknData("PLUS", @"\+"),
            new TknData("MINUS", @"-"),
            new TknData("MUL", @"\*"),
            new TknData("DIV", @"/"),
            new TknData("L_PAREN", @"\("),
            new TknData("R_PAREN", @"\)")
        };

        public LexerResult MakeTokens(string text) {
            int pos = 0;
            List<Token> tokens = new List<Token>();
            bool found;
            while (pos < text.Length) {
                found = false;
                foreach (TknData token in this.tokData) {
                    Match match = Regex.Match(text.Substring(pos), token.regex);
                    if (match.Success & match.Index == 0) {
                        if (token.hasValue) {
                            tokens.Add(new Token(token.name, Convert.ToDecimal(match.Value)));
                        } else {
                            tokens.Add(new Token(token.name));
                        }
                        pos += match.Length;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                	tokens.Add(new Token("EOF"));
                    return new LexerResult(tokens, new Error("Syntax error", text.Substring(pos)));
                }
            }
            tokens.Add(new Token("EOF"));
            return new LexerResult(tokens);
        }
    }
}
