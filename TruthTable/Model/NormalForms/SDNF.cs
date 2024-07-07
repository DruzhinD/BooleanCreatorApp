using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthTable.Model.NormalForms
{
    public class SDNF : INormalizationForm
    {
        public string GetNormalForm(ArgValuePair[] table)
        {
            int amountOfVars = (int)Math.Log(table.Length, 2);

            List<string> expression = new List<string>();
            //создание СДНФ
            for (int i = 0; i < table.Length; i++)
            {
                //пропускаем итерацию, если при текущем наборе функция = 0
                if (table[i].value == 0)
                    continue;

                string[] subExpression = new string[amountOfVars];
                for (int j = 0; j < amountOfVars; j++)
                {
                    if (table[i].args[j] == 0)
                        subExpression[j] = "¬x" + (j+1);
                    else
                        subExpression[j] = "x" + (j+1);
                }
                expression.Add($"({string.Join("∧", subExpression)})");
            }
            if (expression.Count > 0)
                return string.Join("∨", expression);
            else
                throw new ArgumentException("Функция задана неверно. Возможно отсутствуют значения 1");
        }
    }
}
