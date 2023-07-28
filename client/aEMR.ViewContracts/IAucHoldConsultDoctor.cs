using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
/*
* 20230712 #001 DatTB: Thêm trường phân nhóm CLS cho BS đọc kết quả
*/
namespace aEMR.ViewContracts
{
    public interface IAucHoldConsultDoctor
    {
        long StaffID { get; set; }

        string StaffName { get; set; }

        long StaffCatType { get; set; }

        bool IsMultiStaffCatType { get; set; }

        ObservableCollection<long> StaffCatTypeList { get; set; }

        void setDefault(string DefaultStaffName = null);
        string CertificateNumber { get; set; }
        bool IsDoctorOnly { get; set; }
        bool ShowStopUsing { get; set; }
        long CurrentDeptID { get; set; }

        long PCLResultParamImpID { get; set; }

        string StaffCode { get; set; }
        string StaffPrefix { get; set; }
        //▼==== #001
        long PCLResultParamImpIDForDoctor { get; set; }
        //▲==== #001
    }
    public interface IDiagnosisTextBox
    {
        PatientPCLRequest CurrentPclRequest { get; set; }
    }
}
