   M S B A M L   `   `   `  ���� K  GSearchDuplicates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SearchDuplicatesSearchDuplicates  !    SearchDuplicates.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -myclr-namespace:SearchDuplicates     -     � Title$  Search Duplicates�� � Icon$0 */SearchDuplicates;component/Images/a.b.JPG�� � 
ResizeMode$ CanMinimize=� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ ThreeDBorderWindow=�$	��350��$	��499��.��� $	��320��$	��486��.+��� -   $��	txtResult��$	��199��$
��Left=�$��
12,103,0,0q�$�� ��$	��Top=�$	��401���� -   $��	groupBox1��$��52��$
��Left=�$��6,12,0,0q�$	��Top=�$	��468��� $��6,12,18,256q� � ColumnDefinitions �� $��8* ��� $
��115* ��� $
��339* �.+��� -   $��	btnSearch��$%��Поиск дубликатов��$��23��$
��Left=�$��0,15,0,0q�$	��Top=�$	��115��$��2��$��1���� -   $��	btnCancel��$��Отмена��$��23��$
��Left=�$��	13,15,0,0q�$	��Top=�$��75��$��2���� -   $��
btnSaveLog��$��Сохранить лог��$��23��$��0,15,6,0q�$	��Top=�$��2��$��Right=�$��94���� -   $��	btnDelete��$'��!Удалить дубликаты��$��2��$��23��$��Right=�$��
0,15,119,0q�$	��Top=�$	��114��{�.  �� -   $��lblLog��$"��Лог выполнения:��$��25��$
��Left=�$��	12,71,0,0q�$	��Top=�$	��104��$��Normal&��� -	   $��	groupBox2��$	��211��$��	6,97,12,0q�$	��Top=�.��� $	��456��$	��206�� � RowDefinitions �� $
��129* ��� $	��77* � �� $	��20* ��� $
��447* �.+�   SearchDuplicates.GifImage  -
     Name$ 
imgLoading��$��1��$��51��$
��Left=�$��
189,74,0,0q�$
��Fill=�$	��Top=�$��55��   	GifSource$4 ./SearchDuplicates;component/Images/loading.gif��	 J� IsHitTestVisible	 .  