using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanBan
{
    public class KanBanViewModel : INotifyPropertyChanged
    {
        KanBanControl _kanBanControl = null;

        internal KanBanViewModel(KanBanControl kanBanControl)
        {
            _kanBanControl = kanBanControl;
            ShowAddColumnChildWindow_Command = new ShowAddColumnChildWindowCommand(_kanBanControl);
        }

        private ObservableCollection<ColumnViewModel> _columns;
        public ObservableCollection<ColumnViewModel> Columns
        {
            get { return _columns; }
            set { _columns = value; OnPropertyChanged("Columns"); }
        }


        public ICommand ShowAddColumnChildWindow_Command;



        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class ShowAddColumnChildWindowCommand : ICommand
    {
        KanBanControl _kanBanControl = null;

        internal ShowAddColumnChildWindowCommand(KanBanControl kanBanControl)
        {
            _kanBanControl = kanBanControl;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var childWindow = new EditKanBanColumnChildWindow();
            KanBanColumn newColumn = new KanBanColumn() { Header = "", Id = "" };
            childWindow.DataContext = newColumn;
            childWindow.Closed += EditKanBanColumnChildWindow_Closed;
            childWindow.Show();
        }

        private void EditKanBanColumnChildWindow_Closed(object sender, EventArgs e)
        {
            EditKanBanColumnChildWindow cw = (EditKanBanColumnChildWindow)sender;
            if (cw.DialogResult == true)
            {
                //Get the newly created Column's informations:
                KanBanColumn newColumn = (KanBanColumn)cw.DataContext;
                if(_kanBanControl.ColumnHeaderTemplate != null)
                {
                    newColumn.HeaderTemplate = _kanBanControl.ColumnHeaderTemplate;
                }
                //Add the newly created Column to the columns of the KanBanControl and force a refresh
                var columns = _kanBanControl.Columns;
                _kanBanControl.Columns = null;
                columns.Add(newColumn);
                _kanBanControl.Columns = columns;
            }
        }
    }

}
