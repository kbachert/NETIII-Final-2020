   M S B A M L   `   `   `  ����I  EDQInventoryApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DQInventoryAppDQInventoryApp  5   	   '    !DQInventoryApp.frmAccountSettings   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:DQInventoryApp     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5      $	��625��6?   $	��310��6N    � Title$ Account Settings��5   	   $	��310��6"   $	��625��6/   .��5	      � 
������� � RowDefinitions 5
   
   �� 5      6   �� 5      6   �� 5      6   �� 5      6   �� 5      6   �� 5      6    � ColumnDefinitions 5   
   �� 5      $	��200 �6   6-   �� 5      6!   .+�5   
   �� $��1��6   $��0��6   $��Right=�5      $��Center=�5      $��10q�6+   $��DemiBold"�5      .��6&   Phone Number:65   �� 5   
   $��2��6   $��0��6   $��Right=�5      $��Center=�5      $��10q�6+   $��DemiBold"�5      .��6&   Email Address:66   �� 5   
   -   5         ��Name$ txtPhoneNumber��5      $��1��5      $��1��6#   $��	10 5 50 5q�63    �� 	MaxLength$ 12��6F   $��Center=�5       $
��Left=�68   5!   d   �� 5"   
   -   5        $ txtEmailAddress��5"      $��2��5#      $��1��6#   $��	10 5 50 5q�63   $��Center=�5$      $
��Left=�68   6\   �� 5%   
   -   5         ��Name$ btnChangePassword��5%      $��3��5&      $��1��6   $
��Left=�5'      $��Center=�5(      $��	105 0 0 0q�6+   $��DemiBold"�5)      $��Change Password��6&   $��5q�5*      $	��175��6   6J   �� 5+   
   -   5        $ 	btnSubmit��5+      $��4��5,      $��1��6   $
��Left=�5-      
������$��Center=�5.      $��10q�6+   $��DemiBold"�5/      $��Submit��6&   $��5q�50      $	��175��6   6B   �� 51   
   -   5        $ 	btnCancel��51      $��4��52      $��1��6   $
��Left=�53      
�������$��Center=�54      $��	200 0 0 0q�6+   $��DemiBold"�55      $��Cancel��6&   $��5q�56      $	��175��6   6B   57      58      