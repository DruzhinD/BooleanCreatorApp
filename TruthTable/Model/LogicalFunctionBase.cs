using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthTable.Model.NormalForms;

namespace TruthTable.Model
{
    public abstract class LogicalFunctionBase
    {
        /// <summary>
        /// базовое выражение
        /// </summary>
        protected string inFix;
        /// <summary>
        /// Логическая функция 
        /// </summary>
        public abstract string InFix { get; }

        //Список переменных отсортированных по алфавиту
        public string[] variables { get; protected set; }

        protected LogicalFunctionBase(string expression)
        {
            inFix = expression;
        }

        #region Операции
        protected byte Disjunction(byte a, byte b) => Math.Max(a, b); //Дизъюнкция
        protected byte Conjunction(byte a, byte b) => Math.Min(a, b); //Конъюнкция
        protected byte Negation(byte a) => (byte)((a + 1) % 2); //Отрицание
        protected byte Implication(byte a, byte b) => Disjunction(Negation(a), b); //Конъюнкция
        protected byte Equivalence(byte a, byte b)  //Эквивалентность
        {
            if (a == b)
                return 1;
            return 0;
        }
        protected byte PierArrow(byte a, byte b) => Negation(Disjunction(a, b));//стрелка пирса: НЕ ИЛИ
        protected byte SchaefferTouch(byte a, byte b) => Negation(Conjunction(a, b));//штрих шеффера: НЕ И
        #endregion

        //приоритет операторов
        protected byte GetPriority(string op)
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

        protected bool IsOperator(string symb)
        {
            return Operators.Contains(symb);
        }
    }
}
