using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using TruthTable.ViewModel;

namespace TruthTable.OtherControls
{
    //textbox, у которого можно отследить позицию курсора
    //а также отследить и установить фокус
    public class ExtendedTextBox : TextBox
    {
        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.Register(nameof(CaretPosition), typeof(int), typeof(ExtendedTextBox),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCaretPositionChanged));

        public int CaretPosition
        {
            get { return (int)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }

        public ExtendedTextBox()
        {
            //для позиции каретки
            SelectionChanged += (s, e) => CaretPosition = CaretIndex;
            //для фокуса
            this.GotFocus += (s, e) => IsFocusedExtended = true;
            this.LostFocus += (s, e) => IsFocusedExtended = false;
        }

        private static void OnCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ExtendedTextBox).CaretIndex = (int)e.NewValue;
        }

        public static readonly DependencyProperty IsFocusedPropertyExtended =
            DependencyProperty.Register(nameof(IsFocusedExtended), typeof(bool), typeof(ExtendedTextBox),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCoerceValue));

        private static void OnCoerceValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                (d as ExtendedTextBox).Focus();
        }

        public bool IsFocusedExtended
        {
            get { return (bool)GetValue(IsFocusedPropertyExtended); }
            set { SetValue(IsFocusedPropertyExtended, value); }
        }
    }
}
