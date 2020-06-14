using System;
using System.Collections.Generic;
using System.Linq;

namespace calculator {

	class Token {

        public string name;
        public int? val;

		public Token(string name, int? val = null) {
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

    class LexerResult {

        public List<Token> tokens;
        public dynamic error;

        public LexerResult(List<Token> tokens, dynamic error = null) {
            this.tokens = tokens;
            this.error = error;
        }
    }

    class Lexer {

        static string DIGITS = "0123456789";
        static Dictionary<char, string> SIMPLE_TT = new Dictionary<char, string>()
        {
            {'+', "PLUS"},
            {'-', "MINUS"},
            {'*', "MUL"},
            {'/', "DIV"},
            {'(', "L_PAREN"},
            {')', "R_PAREN"},
            {'^', "EXP"},
        };
        static string TT_INT = "INT";

        string text;
        int pos;
        char currentChar;

        public Lexer(string text) {
            this.text = text;
            this.pos = -1;
            this.currentChar = '\0';
            this.Advance();
        }

        void Advance() {
            this.pos ++;
            if (this.pos < this.text.Length) {
                this.currentChar = this.text[this.pos];
            } else {
                this.currentChar = '\0';
            }
        }

        public LexerResult MakeTokens() {
            List<Token> tokens = new List<Token>();

            while (this.currentChar != '\0') {

                // whitespace
                if (this.currentChar == ' ') {
                    this.Advance();
                    if (this.currentChar == '\0') {
                        throw new ArgumentException("Invalid Syntax");
                    }
                }

                // operators
                else if (SIMPLE_TT.ContainsKey(this.currentChar)) {
                    char p = this.currentChar;
                    tokens.Add(new Token(SIMPLE_TT[this.currentChar]));
                    this.Advance();

                    // implicit multiply with brackets
                    string t = "0123456789(";
                    if (p == ')' & this.currentChar != '\0' & t.Contains(this.currentChar)) {
                        tokens.Add(new Token("MUL"));
                    }
                }

                // numbers
                else if (DIGITS.Contains(this.currentChar)) {
                    string num = "";
                    while (this.currentChar != '\0' & DIGITS.Contains(this.currentChar)) {
                        num += this.currentChar;
                        this.Advance();
                    }
                    tokens.Add(new Token(TT_INT, int.Parse(num)));

                    // implicit multiply with brackets
                    if (this.currentChar == '(') {
                        tokens.Add(new Token("MUL"));
                    }
                }

                else {
                    return new LexerResult(tokens, new Error("Invalid Character", this.currentChar.ToString()));
                }
            }

            tokens.Add(new Token("EOF"));
            return new LexerResult(tokens);
        }
    }
}
