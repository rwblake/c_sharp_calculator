using System;
using System.Collections.Generic;
using System.Linq;

namespace calculator {

	class NumberNode {

		public Token tok;

		public NumberNode(Token tok) {
			this.tok = tok;
		}

		public string Repr() {
			return $"{this.tok.Repr()}";
		}

		public decimal Eval() {
			return (decimal)this.tok.val;
		}
	}

	class BinOpNode {

		public Token opTok;
		public dynamic lNode;
		public dynamic rNode;

		public BinOpNode(Token opTok, dynamic lNode, dynamic rNode) {
			this.opTok = opTok;
			this.lNode = lNode;
			this.rNode = rNode;
		}

		public string Repr() {
			return $"({this.lNode.Repr()}, {this.opTok.Repr()}, {this.rNode.Repr()})";
		}

		public decimal Eval() {
			switch (this.opTok.name) {
				case "PLUS":  return this.lNode.Eval() + this.rNode.Eval();
				case "MINUS": return this.lNode.Eval() - this.rNode.Eval();
				case "MUL":   return this.lNode.Eval() * this.rNode.Eval();
				case "DIV":   return this.lNode.Eval() / this.rNode.Eval();
				case "EXP":   return (decimal)Math.Pow((double)this.lNode.Eval(), (double)this.rNode.Eval());
				default:      throw new ArgumentException("Unknown operator token", this.opTok.name);
			}
		}
	}

	class UnOpNode {

		public Token opTok;
		public dynamic node;

		public UnOpNode(Token opTok, dynamic node) {
			this.opTok = opTok;
			this.node = node;
		}

		public string Repr() {
			return $"({this.opTok.Repr()}, {this.node.Repr()})";
		}

		public decimal Eval() {
			switch (this.opTok.name) {
				case "PLUS":  return this.node.Eval();
				case "MINUS": return this.node.Eval() * -1;
				default:      throw new ArgumentException("Unknown operator token", this.opTok.name);
			}
		}
	}

	class ParseResult {

		public dynamic error;
		public dynamic node;

		public ParseResult() {
			this.error = null;
			this.node = null;
		}

		public dynamic Register(dynamic res) {
			if (res is ParseResult) {
				if (res.error != null) {
					this.error = res.error;
				}
				return res.node;
			}
			return res;
		}

		public ParseResult Success(dynamic node) {
			this.node = node;
			return this;
		}

		public ParseResult Failure(dynamic error) {
			this.error = error;
			return this;
		}
	}

	class Parser {

		List<Token> tokens = new List<Token>();
		int pos;
		Token currentTok;

		public Parser(List<Token> tokens) {
			this.tokens = tokens;
			this.pos = -1;
			this.Advance();
		}

		Token Advance() {
			this.pos ++;
			if (this.pos < this.tokens.Count) {
				this.currentTok = this.tokens[this.pos];
			}
			return this.currentTok;
		}

		public ParseResult Parse() {
			dynamic res = this.Expr();
			if (res.error == null & this.currentTok.name != "EOF") {
				return res.Failure(new Error("Invalid syntax", this.currentTok.Repr()));
			}
			return res;
		}

		dynamic Factor() {
			ParseResult res = new ParseResult();
			Token tok = this.currentTok;

			if (tok.name == "PLUS" | tok.name == "MINUS") {
				res.Register(this.Advance());
				dynamic factor = res.Register(this.Factor());
				if (res.error != null) {
					return res;
				}
				return res.Success(new UnOpNode(tok, factor));
			}

			else if (tok.name == "INT") {
				res.Register(this.Advance());
				return res.Success(new NumberNode(tok));
			}

			else if (tok.name == "L_PAREN") {
				res.Register(this.Advance());
				dynamic expr = res.Register(this.Expr());
				if (res.error != null) {
					return res;
				}
				if (this.currentTok.name == "R_PAREN") {
					res.Register(this.Advance());
					return res.Success(expr);
				} else {
					return res.Failure(new Error("Expected closing parenthesis", this.currentTok.Repr()));
				}
			}

			else {
				return res.Failure(new Error("Expected int", this.currentTok.Repr()));
			}
		}

		ParseResult BinOp(Func<dynamic> outerFunc, List<string> ops) {
			dynamic r;
			ParseResult res = new ParseResult();
			dynamic l = res.Register(outerFunc());
			if (res.error != null) {
				return res;
			}
			while (ops.Contains(this.currentTok.name)) {
				Token op = this.currentTok;
				res.Register(this.Advance());
				r = res.Register(outerFunc());
				if (res.error != null) {
					return res;
				}
				l = new BinOpNode(op, l, r);
			}
			return res.Success(l);
		}

		ParseResult Exp() {
			return this.BinOp(this.Factor, new List<string>(){"EXP"});
		}

		ParseResult Term() {
			return this.BinOp(this.Exp, new List<string>(){"MUL", "DIV"});
		}

		ParseResult Expr() {
			return this.BinOp(this.Term, new List<string>(){"PLUS", "MINUS"});
		}
	}
}
