   M S B A M L   `   `   `  ����K  GSearchDuplicates, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SearchDuplicatesSearchDuplicates  5   b  !    SearchDuplicates.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   4   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6q   -myclr-namespace:SearchDuplicates     6b  -   5          � Title$  Search Duplicates��5   	    � Icon$0 */SearchDuplicates;component/Images/a.b.JPG��6�    � 
ResizeMode$ CanMinimize=�6�    � WindowStartupLocation$ CenterScreen=�6   � WindowStyle$ ThreeDBorderWindow=�6(  $	��350��6I  $	��499��6V  .��5      � $	��320��6   $	��486��6   .+�5   
   �� -   5        $��	txtResult��5   P   $	��199��6   $
��Left=�6!   $��
12,103,0,0q�6<   $�� ��6a   $	��Top=�6i   $��202.529��6�    J� IsManipulationEnabled .  � UseLayoutRounding .  6�   �� 5   
   -   5        $��	groupBox1��5   L   $��52��6   $
��Left=�6   $��6,12,0,0q�6:   $	��Top=�6]   $	��468��6u   6�   � 5   
   $��6,12,18,256q�6    � ColumnDefinitions 5	      �� 5
      $��8* �6#   �� 5      $	��84* �6#   �� 5      $
��387* �6#   .+�5      �� -   5        $��	btnSearch��5   i   $%��Поиск дубликатов��6   $��23��60   $
��Left=�6<   $��6,15,0,0q�6W   $	��Top=�6z   $	��104��6�   $��3��6�   6�   �� 5      -   5        $��	btnCancel��5   `   $��Отмена��6   $��23��6&   $
��Left=�62   $��	38,15,0,0q�6M   $	��Top=�6q   $��75��6�   $��2��6�   6�   �� 5      -   5        $��
btnSaveLog��5   N   $��Сохранить лог��6   $��23��6-   $��265,15,26,0q�69   $	��Top=�6`   $��2��6x   6�   5      �� 5   
   -   5        $��lblLog��5   d   $"��Лог выполнения:��6   $��21��6*   $
��Left=�66   $��	12,72,0,0q�6Q   $	��Top=�6r   $��97��6�   $��Normal&�6�   �� 5   
   -   5        $��	groupBox2��5   3   $	��211��6   $��	6,97,12,0q�6    $	��Top=�6D   .��5      � $	��456��6   $	��206��6    5      �� 5      $	��20* �6'   �� 5      $
��447* �6'   .+�5         SearchDuplicates.GifImage  -	   5          Name$ 
imgLoading��5   i   $��1��6   $��51��6.   $
��Left=�6:   $��
189,74,0,0q�6U   $
��Fill=�6}   $	��Top=�6�   $��55��6�   	   	GifSource$4	 ./SearchDuplicates;component/Images/loading.gif��6�   
 J� IsHitTestVisible
 .  6�   5      5      5      5      