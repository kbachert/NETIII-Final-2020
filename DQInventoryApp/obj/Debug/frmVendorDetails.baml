   M S B A M L   `   `   `  ����I  EDQInventoryApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DQInventoryAppDQInventoryApp  5   	   %    DQInventoryApp.frmVendorDetails   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:DQInventoryApp     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5	   	   $	��500��6.   $	��300��6=    � Title$ Vendor Details��5
   	   $	��600��6    .��5      � 
������� � RowDefinitions 5   
   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*    � ColumnDefinitions 5   
   �� 5      6!   �� 5      6!   .+�5   
   �� $��1��6   $��Center=�6   $��DemiBold"�68   $��Right=�5      $��0,8,10,7q�6,   $��0��6>   .��6N   Vendor Name6[   �� 5   
   $��2��6   $��Center=�6   $��DemiBold"�68   $��Right=�5      $��0,7,10,8q�6,   $��0��6>   .��6N   Phone Number6\   �� 5   
   $��3��6   $��Center=�6   $��DemiBold"�68   $��Right=�5      $��0 0 10 0q�6,   .��6>   Active6F   �� 5   
   -   5         ��Name$ txtVendorName��5      $��1��6)   $��1��66   $
��10 0q�6F   $��Center=�5      $��0 5 40 5q�64   6H   �� 5    
   -   5        $ txtPhoneNumber��5       $��2��6*   $��1��67   $
��10 0q�6G   $��Center=�5!      $��0 5 40 5q�64    �� 	MaxLength$ 12��6F   5"   d   �� 5#   
   -   5         ��Name$ 	chkActive��5#      $��Center=�6&   $��3��5$      $��1��6    62   �� 5%   
   -   5         ��Name$ btnEdit��5%      $��4��6"   $��0,5,5,5q�6/   $
��Edit��6@   $��DemiBold"�6O   $	��100��5&      $��Right=�6   $��0��6P   6b   �� 5'   
   -   5        $ btnSave��5'      $��4��6"   $��0,5,5,5q�6/   $
��Save��6@   $��DemiBold"�6O   $	��100��5(      $��Right=�6   $y�Hidden=�69   
������$��0��5)   )   6;   �� 5*   
   -   5        $ 	btnCancel��5*      $��4��6$   $��1��61   $��5,5,0,5q�6A   $��Cancel��6R   $	��100��5+      $
��Left=�6   $��DemiBold"�68   
�������5,   +   5-      5.      