using System;
using System.Collections.Generic;
using System.Linq;

namespace calculator {

    class Program {

        static void Main(string[] args) {
            string text;
            Lexer l;
            Parser p;
            decimal res;
            ParseResult p_res;
            LexerResult l_res;

            while (true) {
                Console.Write("calc > ");
                text = Console.ReadLine().Trim();

                if (text == "quit()") {
                    return;
                }

                else if (text == "") {
                    continue;
                }

                l = new Lexer(text);
                l_res = l.MakeTokens();

                if (l_res.error != null) {
                    Console.WriteLine(l_res.error.Repr());
                    continue;
                }

                p = new Parser(l_res.tokens);
                p_res = p.Parse();

                if (p_res.error != null) {
                    Console.WriteLine(p_res.error.Repr());
                    continue;
                }

                res = p_res.node.Eval();
                Console.WriteLine(res);
            }
        }
    }

    class Error {

        string name;
        string arg;

        public Error(string name, string arg) {
            this.name = name;
            this.arg = arg;
        }

        public string Repr() {
            return $"{this.name}, \'{this.arg}\'";
        }
    }
}
