using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TruthTable.Model
{
    public class LogicalFunction
    {
        string inFix;
        public string InFix { get => inFix; }

        string[] postFix;

        //Список переменных отсортированных по алфавиту
        public readonly string[] variables;

        //реализовать проверку
        public LogicalFunction(string expression)
        {
            inFix = expression;
            postFix = ToPostFix(expression);
            variables = FindVariables();
        }

        //ищет переменные в выражении
        private string[] FindVariables()
        {
            SortedSet<string> variables = new SortedSet<string>();

            for (int i = 0; i < postFix.Length; i++)
            {
                if (!IsOperator(postFix[i]))
                {
                    postFix[i] = postFix[i].ToLower();
                    variables.Add(postFix[i]);
                }
            }
            return variables.ToArray();
        }

        //приоритет операторов
        private byte GetPriority(string op)
        {
            switch (op)
            {
                case "(":
                    return 0;
                case ")":
                    return 0;
                case "↓":
                    return 1;
                case "↑":
                    return 1;
                case "~":
                    return 2;
                case ">":
                    return 3;
                case "<":
                    return 3;
                case "+":
                    return 4;
                case "*":
                    return 5;
                case "!":
                    return 6;
                default:
                    return 7;
            }
        }

        //есть ли смысл в этом свойстве?
        public string[] Operators
        {
            get => new string[] { "(", ")", "~", ">", "<", "+", "*", "!", "↓", "↑" };
        }

        //из инфиксной в постфиксную
        private string[] ToPostFix(string expression)
        {
            //будет содержать в себе конечное выражение в постфиксной форме
            List<string> postFix = new List<string>();

            //парсим выражение
            Regex regex = new Regex(@"([a-zA-Z0-9]+)|([()~><+*!↑↓]{1})");
            MatchCollection mathes = regex.Matches(expression);

            //копируем распаршенное выражение в массив
            //необходимо для того, чтобы была возможность работать с переменными, состоящими из нескольких символов
            string[] arrayExpr = new string[mathes.Count];
            for(int i = 0; i < mathes.Count; i++)
                arrayExpr[i] = mathes[i].Value;

            Stack<string> stackForOperations = new Stack<string>(); //Стек для хранения операторов

            for (int i = 0; i < arrayExpr.Length; i++)
            {
                //Если символ - буква, то добавляем к строке хранения выражения
                if (!IsOperator(arrayExpr[i]))
                {
                    postFix.Add(arrayExpr[i]);
                }
                //Если символ - оператор
                else if (IsOperator(arrayExpr[i]))
                {
                    if (arrayExpr[i] == "(")
                    {
                        stackForOperations.Push(arrayExpr[i]); //Записываем открывающую скобку в стек
                    }
                    else if (arrayExpr[i] == ")")
                    {
                        //Выписываем все операторы до открывающей скобки в строку
                        string s = stackForOperations.Pop();
                        while (s != "(")
                        {
                            postFix.Add(s);
                            s = stackForOperations.Pop();
                        }
                    }
                    else //Если любой другой оператор
                    {
                        if (stackForOperations.Count > 0) //Если в стеке есть элементы
                        {
                            //И если приоритет нашего оператора меньше или равен приоритету оператора на вершине стека
                            if (GetPriority(arrayExpr[i]) <= GetPriority(stackForOperations.Peek()) && arrayExpr[i] != "!")
                                //То добавляем последний оператор из стека в выражение
                                postFix.Add(stackForOperations.Pop());
                        }
                        //Если стек пуст, или же приоритет оператора выше - добавляем операторов на вершину стека
                        stackForOperations.Push(arrayExpr[i]); 
                    }
                }
            }

            //Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (stackForOperations.Count > 0)
            {
                postFix.Add(stackForOperations.Pop());
            }

            return postFix.ToArray();
        }
        private bool IsOperator(string symb)
        {
            return Operators.Contains(symb);
        }

        #region Операции

        private byte Disjunction(byte a, byte b) => Math.Max(a, b); //Дизъюнкция
        private byte Conjunction(byte a, byte b) => Math.Min(a, b); //Конъюнкция
        private byte Negation(byte a) => (byte)((a + 1) % 2); //Отрицание
        private byte Implication(byte a, byte b) => Disjunction(Negation(a), b); //Конъюнкция
        private byte Equivalence(byte a, byte b)  //Эквивалентность
        {
            if (a == b)
                return 1;
            return 0;
        }
        private byte PierArrow(byte a, byte b) => Negation(Disjunction(a, b));//стрелка пирса: НЕ ИЛИ
        private byte SchaefferTouch(byte a, byte b) => Negation(Conjunction(a, b));//штрих шеффера: НЕ И
        #endregion

        public byte FunctionValue(byte[] argsValues)
        {
            if (argsValues.Length != variables.Length)
                throw new ArgumentException("Неверно задано количество аргументов.");

            foreach (var v in argsValues)
                if (v != 0 && v != 1)
                    throw new ArgumentException($"Неверно задано значение аргумента. Получено {v}, ожидалось 0|1");

            Dictionary<string, byte> variableValue = new Dictionary<string, byte>(); //хранит текущее значение аргументов
            for (int i = 0; i < variables.Length; i++)
                variableValue.Add(variables[i], argsValues[i]);

            byte result;
            Stack<byte> stack = new Stack<byte>(); //стек для хранения промежуточных результатов

            for (int i = 0; i < postFix.Length; i++)
            {
                //Если символ - буква, то записываем на вершину стека
                if (!IsOperator(postFix[i]))
                {
                    stack.Push(variableValue[postFix[i]]);
                }
                else if (IsOperator(postFix[i])) //Если символ - оператор
                {
                    //Берем одно значение из стека
                    byte a = stack.Pop();
                    byte b;

                    switch (postFix[i]) //И производим над ними действие, согласно оператору
                    {
                        case "+":
                            b = stack.Pop();
                            result = Disjunction(b, a);
                            stack.Push(result);
                            break;
                        case "*":
                            b = stack.Pop();
                            result = Conjunction(b, a);
                            stack.Push(result);
                            break;
                        case "~":
                            b = stack.Pop();
                            result = Equivalence(b, a);
                            stack.Push(result);
                            break;
                        case ">":
                            b = stack.Pop();
                            result = Implication(b, a);
                            stack.Push(result);
                            break;
                        case "<":
                            b = stack.Pop();
                            result = Implication(a, b);
                            stack.Push(result);
                            break;
                        case "!":
                            result = Negation(a);
                            stack.Push(result);
                            break;
                        case "↑":
                            b = stack.Pop();
                            result = SchaefferTouch(b, a);
                            stack.Push(result);
                            break;
                        case "↓":
                            b = stack.Pop();
                            result = PierArrow(b, a);
                            stack.Push(result);
                            break;
                    }
                }
            }
            return stack.Peek(); //Забираем результат всех вычислений из стека и возвращаем его
        }
    }
}
