   M S B A M L   `   `   `  ����I  EDQInventoryApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DQInventoryAppDQInventoryApp  5   	   &     DQInventoryApp.frmUpdatePassword   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:DQInventoryApp     5   	   -   5          � Title$  Update Password��5   	   $	��310��6!   $	��500��6.   $	��500��6:   $	��310��6I    � WindowStartupLocation$ CenterScreen=�5	   	   .��5
      � 
������� � RowDefinitions 5   
   �� 5      6   �� 5      6   �� 5      6   �� 5      6   �� 5      6   �� 5      6    � ColumnDefinitions 5   
   �� 5      $	��200 �6   6-   �� 5      6!   .+�5   
   �� $��1��6   $��0��6   $��Right=�5      $��Center=�5      $��10q�6+   $��DemiBold"�5      .��6&   Current Password:69   �� 5   
   $��2��6   $��0��6   $��Right=�5      $��Center=�5      $��10q�6+   $��DemiBold"�5      .��6&   New Password:65   �� 5   
   $��3��6   $��0��6   $��Right=�5       $��Center=�5!      $��10q�6+   $��DemiBold"�5"      .��6&   Retype Password:68   �� 5#   
   -   5         ��Name$ 	btnSubmit��5#      $��4��5$      $��1��6   $
��Left=�5%      
������$��Center=�5&      $��10q�6+   $��DemiBold"�5'      $��Submit��6&   $��5q�5(      $	��175��6   6B   G� 5)   
   -   5         G�Name$ pwdCurrentPassword��5)      $��1��5*      $��1��6#   $��	10 5 50 5q�63   $��Center=�5+      $��Center=�68   6^   G� 5,   
   -   5        $ pwdNewPassword��5,      $��2��5-      $��1��6#   $��	10 5 50 5q�63   $��Center=�5.      $��Center=�68   6^   G� 5/   
   -   5        $ pwdRetypePassword��5/      $��3��50      $��1��6#   $��	10 5 50 5q�63   $��Center=�51      $��Center=�68   6^   52      53      