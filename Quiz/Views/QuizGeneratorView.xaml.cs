﻿using Quiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quiz.Views
{
    /// <summary>
    /// Logika interakcji dla klasy QuizGeneratorView.xaml
    /// </summary>
    public partial class QuizGeneratorView : UserControl
    {
        public QuizGeneratorView()
        {
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is QuizGeneratorViewModel viewModel && viewModel.EditQuizCommand.CanExecute(null))
            {
                viewModel.EditQuizCommand.Execute(null);
            }
        }
    }
}
