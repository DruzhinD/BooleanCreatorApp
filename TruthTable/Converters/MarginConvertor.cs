using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace TruthTable.Converters
{
    public class MarginConvertor : IValueConverter
    {
        //parameter:
        //коэф ширины; коэф высоты; столбец; строка
        //при этом если строка или столбец == -1, то параметр игнорируется в дальнейшем
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] param = parameter.ToString().Split(';');
            DementionGetter dementionGetter = null;
            switch (value.GetType().Name)
            {
                case "Grid":
                    dementionGetter = new GridDementionGetter((Grid)value);
                    break;
                case "UniformGrid":
                    dementionGetter = new UniformGridDementionGetter((UniformGrid)value);
                    break;
            }

            //ширина длина
            (double, double) dementions = (dementionGetter.GetWidth(param), dementionGetter.GetHeight(param));
            (double, double) factors = (double.Parse(param[0]), double.Parse(param[1]));
            Thickness thickness = new Thickness()
            {
                Left = dementions.Item1 * factors.Item1,
                Right = dementions.Item1 * factors.Item1,
                Top = dementions.Item2 * factors.Item2,
                Bottom = dementions.Item2 * factors.Item2,
            };

            return thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object[] objects = new object[1];
            objects[0] = DependencyProperty.UnsetValue;
            return objects;
        }
    }

    internal abstract class DementionGetter
    {
        //ширина у столбца
        public abstract double GetWidth(string[] param);
        //высота у строки
        public abstract double GetHeight(string[] param);
    }

    internal class GridDementionGetter : DementionGetter
    {
        private Grid container;
        public GridDementionGetter(Grid container) => this.container = container;

        public override double GetHeight(string[] param)
        {
            int row = int.Parse(param[3]);
            double height = container.RowDefinitions[row].ActualHeight;
            return height;
        }

        public override double GetWidth(string[] param)
        {
            int column = int.Parse(param[2]);
            double width;
            if (column != -1)
                width = container.ColumnDefinitions[column].ActualWidth;
            else
                width = container.ActualHeight;
            return width;
        }
    }

    internal class UniformGridDementionGetter : DementionGetter
    {
        private UniformGrid container;
        public UniformGridDementionGetter(UniformGrid container) => this.container = container;
        public override double GetHeight(string[] param)
        {
            double height = container.ActualHeight / container.Rows;
            return height;
        }

        public override double GetWidth(string[] param)
        {
            double width = container.ActualWidth / container.Columns;
            return width;
        }
    }
}
