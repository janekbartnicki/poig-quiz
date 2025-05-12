using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quiz.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ustawia wartość właściwości i powiadamia o zmianie, jeśli wartość faktycznie się zmieniła.
        /// </summary>
        /// <typeparam name="T">Typ właściwości</typeparam>
        /// <param name="storage">Referencja do pola przechowującego wartość właściwości</param>
        /// <param name="value">Nowa wartość</param>
        /// <param name="propertyName">Nazwa właściwości (automatycznie uzupełniana przez kompilator)</param>
        /// <returns>True, jeśli wartość została zmieniona; false w przeciwnym razie</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Wywołuje zdarzenie PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nazwa właściwości, która się zmieniła</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}