using System;
using System.Collections.Generic;
using System.Linq;

namespace calculator {

    class Program {

        static void Main(string[] args) {
            string text;
            ParseResult parseRes;
            LexerResult lexRes;
            decimal result;

            while (true) {
                text = GetText();

                switch (text) {
                    case "quit()": return;
                    case "":       continue;
                }

                lexRes = Lex(text);

                // print error if applicable
                if (lexRes.error != null) {
                    Console.WriteLine(lexRes.error.Repr());
                    continue;
                }

                parseRes = Parse(lexRes.tokens);

                // print error if applicable
                if (parseRes.error != null) {
                    Console.WriteLine(parseRes.error.Repr());
                    continue;
                }

                result = parseRes.node.Eval();
                Console.WriteLine(result);
            }
        }

        static string GetText() {
            Console.Write("calc > ");
            return Console.ReadLine().Trim();
        }

        static LexerResult Lex(string text) {
            Lexer lexer = new Lexer();
            return lexer.MakeTokens(text);
        }

        static ParseResult Parse(List<Token> tokens) {
            Parser parser = new Parser(tokens);
            return parser.Parse();
        }
    }
}
