using System;
using System.Collections.Generic;
using System.IO;

namespace Programming_Compilers_Pascal
{
    public class SyntaxAnalizer
    {
        private List<LexemeData> lexemesData = new List<LexemeData>();
        private int indexCurrentLexeme = 0;
        private LexemeData lexeme = null;

        private string filePath;

        public SyntaxAnalizer(List<LexemeData> lexemesData, string filePath = null)
        {
            this.lexemesData = lexemesData;
            this.filePath = filePath;
        }

        public void Analise()
        {
            if (filePath != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath.Remove(filePath.Length - 4, 4) + "_output.txt"))
                {
                    while (true)
                    {
                        Node node = ParseExpr();
                        if (node == null)
                            break;
                        string[] str = node.Print().Split('\n');

                        for (int i = 0; i < str.Length; i++)
                        {
                            streamWriter.WriteLine(str[i]);
                        }
                        
                    }
                }
            }
            else
            {
                while (true)
                {
                    Node node = ParseExpr();
                    if (node == null)
                        break;
                    Console.WriteLine(node.Print());
                }
            }
        }

        private Node ParseExpr()
        {
            Node left = ParseTerm();
            LexemeData operation = GetCurrentLexeme();
            if (operation != null)
            {
                while (operation.value.Equals("+") | operation.value.Equals("-"))
                {
                    GetNextLexeme();
                    Node right = ParseTerm();
                    left = new BinOperationNode(left, operation, right);
                    operation = GetCurrentLexeme();
                    if (operation == null)
                        break;
                }
            }
            return left;
        }

        private Node ParseTerm()
        {
            Node left = ParseFactor();
            LexemeData operation = GetCurrentLexeme();
            if (operation != null)
            {
                while (operation.value.Equals("*") | operation.value.Equals("/"))
                {
                    GetNextLexeme();
                    Node right = ParseFactor();
                    left = new BinOperationNode(left, operation, right);
                    operation = GetCurrentLexeme();
                    if (operation == null)
                        break;
                }
            }
            return left;
        }

        private Node ParseFactor()
        {
            lexeme = GetCurrentLexeme();
            GetNextLexeme();
            if (lexeme == null)
                return null;
            if (lexeme.classLexeme == ClassLexeme.variable | lexeme.classLexeme == ClassLexeme.integer | lexeme.classLexeme == ClassLexeme.real | lexeme.classLexeme == ClassLexeme.@string)
                return new LexemeNode(lexeme);
            if (lexeme.value.Equals('('))
            {
                Node left = ParseExpr();
                if (lexeme.value.Equals(')'))
                {
                    return null;
                }

                return left;
            }

            return null;
        }

        private LexemeData GetCurrentLexeme()
        {
            if (indexCurrentLexeme >= lexemesData.Count)
                return null;
            return lexemesData[indexCurrentLexeme];
        }

        private LexemeData GetNextLexeme()
        {
            indexCurrentLexeme++;
            if (indexCurrentLexeme >= lexemesData.Count)
                return null;
            return lexemesData[indexCurrentLexeme];
        }
    }

    public abstract class Node
    {
        public abstract string Print(int level = 1);
    }

    public class LexemeNode : Node
    {
        public LexemeData lexemeData;
        public LexemeNode(LexemeData lexemeData)
        {
            this.lexemeData = lexemeData;
        }

        public override string Print(int level = 1)
        {
            return lexemeData.value;
        }
    }

    public class BinOperationNode : Node
    {
        public Node left;
        public LexemeData operation;
        public Node right;

        public BinOperationNode(Node left, LexemeData operation, Node right)
        {
            this.left = left;
            this.operation = operation;
            this.right = right;
        }

        public override string Print(int level = 1)
        {
            string space = null;
            for (int i = 0; i < level; i++)
                space += " ";

            string right = null;
            string left = null;

            if (this.right != null)
                right = this.right.Print(level + 1);
            if (this.left != null)
                left = this.left.Print(level + 1);

            if (this.right != null & this.left != null)
                return $"{operation.code}" + '\n' + $"{space}{left}" + '\n' + $"{space}{right}";
            if (this.right != null & this.left == null)
                return $"{operation.code}" + '\n' + $"{space}{right}";
            if (this.right == null & this.left != null)
                return $"{operation.code}" + '\n' + $"{space}{left}";

            return null;
        }
    }
}

//╓  ╙  ║╓