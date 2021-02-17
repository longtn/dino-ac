using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace AC.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public Page Parent { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase(Page Parent)
        {
            this.Parent = Parent;
        }

        protected void RaisePropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (!string.IsNullOrEmpty(PropertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void SetPropertyValue<D>(ref D Property, D value, [CallerMemberName] string PropertyName = "")
        {
            Property = value;
            RaisePropertyChanged(PropertyName);
        }
    }
}
