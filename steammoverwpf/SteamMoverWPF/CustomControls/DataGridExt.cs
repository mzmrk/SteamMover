using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SteamMoverWPF
{
    public class ValueEventArgs<T> : EventArgs
    {
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; set; }

    }

    public class DataGridExt : DataGrid
    {
        public event EventHandler<ValueEventArgs<DataGridColumn>> Sorted;

        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            base.OnSorting(eventArgs);

            if (Sorted == null) return;
            var column = eventArgs.Column;
            Sorted(this, new ValueEventArgs<DataGridColumn>(column));
        }
    }
}
