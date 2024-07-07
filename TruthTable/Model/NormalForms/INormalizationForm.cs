using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthTable.Model.NormalForms
{
    public interface INormalizationForm
    {
        string GetNormalForm(ArgValuePair[] table);
    }
}
