﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.ViewContracts.Configuration
{
    public interface IRefDepartments
    {
        //void DeleteRefDepartments(DataEntities.RefDepartmentsTree item);
        void DeleteRefDepartments(Int64 DeptID);
        void DeptLocation_MarkDeleted(Int64 DeptLocationID);

        
    }
}
