using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TruthTable.Model;

namespace TruthTable.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region Реализация интерфейса
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        private string inputExpression = "";
        public string InputExpression { get { return inputExpression; }
            set {
                inputExpression = value;
                OnPropertyChanged();
            } }

        private ValueTable valueTable;

        //команда построения функции
        private RelayCommand buildFunction;
        public RelayCommand BuildFunction { get
            {
                return buildFunction ?? (buildFunction = new RelayCommand(obj =>
                {
                    //задаем таблицу значений функции
                    valueTable = new ValueTable(
                        new LogicalFunction(CheckAndRewriteExpression())
                        );
                    //Вызываем метод заполнения DataTable и отображаем datagrid
                    if (IsBuildValueTable)
                    {
                        FillDataGridValueTable();
                        DataGridVisibility = Visibility.Visible;
                    }
                }));
            } }

        //проверка корректности введенной функции и замена операторов на те, что обрабатывает LogicalExpression
        private string CheckAndRewriteExpression()
        {
            //копируем введенное выражение
            StringBuilder expression = new StringBuilder(inputExpression);

            //замена операторов
            (char, char)[] replacingOperators =
            {
                ('\u2194', '~'),
                ('\u2192', '>'),
                ('\u2190', '<'),
                ('\u2228', '+'),
                ('\u2227', '*'),
                ('\u00AC', '!'),
            };
            
            foreach (var replace in replacingOperators)
                expression = expression.Replace(replace.Item1.ToString(), replace.Item2.ToString());

            //проверка корректного ввода
            //some

            return expression.ToString();
        }

        //содержит таблицу значений функции для вывода на экран
        private DataView dataViewValueTable;
        public DataView DataViewValueTable { get => dataViewValueTable; set {
                dataViewValueTable = value;
                OnPropertyChanged(nameof(DataViewValueTable));
            } }

        //заполняет таблицу значений
        private void FillDataGridValueTable()
        {
            //DataView содержит в себе DataTable, поэтому можно работать с DataTable
            DataTable table = new DataTable();

            //добавление столбцов
            for (int i = 0; i < valueTable.AmountOfVariables;  i++)
                table.Columns.Add(new DataColumn(valueTable.Variables[i]));
            //столбец с результатом
            table.Columns.Add(new DataColumn("Result"));

            //добавление строк
            for (int i = 0; i < valueTable.MaxDecimalValue; i++)
            {
                //формирование массива, из которого будет заполнена строка
                List<object> row = new List<object>(valueTable.AmountOfVariables + 1);
                for (int j = 0; j < valueTable.AmountOfVariables; j++)
                    row.Add(valueTable[i].args[j]);
                row.Add((object)valueTable[i].value);

                table.Rows.Add(row.ToArray());
            }
            //формируем DataView на основании DataTable
            DataViewValueTable = table.AsDataView();
        }

        //видимость datagrid
        Visibility dataGridVisibility = Visibility.Hidden;
        public Visibility DataGridVisibility { get => dataGridVisibility; set {
                dataGridVisibility = value;
                OnPropertyChanged();
            } }

        //команда вывода содержимого кнопки в вводимое выражение
        private RelayCommand printSymbol;
        public RelayCommand PrintSymbol { get
            {
                return printSymbol ?? (printSymbol = new RelayCommand(obj =>
                {
                    InsertSymbToInputField((string)obj);
                }));
            } }

        private void InsertSymbToInputField(string symb)
        {
            int cursorPos = cursorPosition; //позиция курсора до вставки символа
            InputExpression = InputExpression.Insert(CursorPosition, symb);
            //указываем позицию с учетом вставленного символа
            CursorPosition = cursorPos + symb.Length;
            //возвращаем фокус в поле ввода
            IsFocused = true;
        }

        //хранит позицию курсора в строке ввода
        private int cursorPosition;
        public int CursorPosition { get => cursorPosition; set
            {
                cursorPosition = value;
                OnPropertyChanged();
            } }

        #region CheckBox'ы, свойства, к которым они привязаны
        //построение таблицы истинности (значений)
        private bool isBuildValueTable;
        public bool IsBuildValueTable { get { return isBuildValueTable; } set
            {
                isBuildValueTable = value;
                OnPropertyChanged();
            } }
        #endregion

        //свойство, необходимое для отслеживания фокуса у поля ввода
        bool _isFocused = false;
        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }

    }
}

