using System;
using System.Linq;

namespace DataEntities
{
    //class PACAPI
    //{
    //}
    public partial class HL7_ITEM
    {
        public HL7_ITEM(PatientPCLRequest aPatientPCLRequest, int? aHeight = 150, double? aWeight = 70)
        {
            if (aPatientPCLRequest.Patient == null
            || aPatientPCLRequest.PatientPCLRequestIndicators == null
            || aPatientPCLRequest.PatientPCLRequestIndicators.Count != 1
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType == null)
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null)
            || (!aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.IsSendToPAC))
            {
                return;
            }
            MaChiDinh = aPatientPCLRequest.PCLRequestNumID;
            ThoiGianChiDinh = aPatientPCLRequest.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            MaBenhNhan = aPatientPCLRequest.Patient.PatientCode;
            TenBenhNhan = aPatientPCLRequest.Patient.FullName;
            NgaySinh = ((DateTime)aPatientPCLRequest.Patient.DOB).ToString("yyyyMMdd");
            GioiTinh = aPatientPCLRequest.Patient.Gender;
            DiaChi = aPatientPCLRequest.Patient.PatientFullStreetAddress;
            SDT = aPatientPCLRequest.Patient.PatientCellPhoneNumber;
            NoiChiDinh = aPatientPCLRequest.RequestedDepartment == null ? aPatientPCLRequest.ReqFromDeptLocIDName : aPatientPCLRequest.RequestedDepartment.DeptName;
            MaBacSiChiDinh = aPatientPCLRequest.RequestedDoctorCode;
            TenBacSiChiDinh = aPatientPCLRequest.RequestedDoctorName ?? aPatientPCLRequest.StaffIDName ?? "";
            MaDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeCode;
            TenDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeName;
            ChanDoan = aPatientPCLRequest.Diagnosis;
            NhomDichVu = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null ? "" : aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.SubCategoryCodeToPAC;
            //PatientClass = aPatientPCLRequest.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? "I" : "O";
            TrangThai = aPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN ? "NEW" : (aPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL ? "CANCEL" : "");
        }

        public string MaChiDinh { get; set; }
        //  2020-10-07 16:08:47.373
        public string ThoiGianChiDinh { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }

        // 19910101
        public string NgaySinh { get; set; }
        // M: Male F: Female
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string NoiChiDinh { get; set; }
        public string MaBacSiChiDinh { get; set; }
        public string TenBacSiChiDinh { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string ChanDoan { get; set; }
        public string NhomDichVu { get; set; }
        // NEW, DELETED, MODIFIED
        public string TrangThai { get; set; }
        // defaul 0
        public string PhanLoai { get; set; }
    }

    public partial class PACResult
    {
        public PACResult(PatientPCLImagingResult aPatientPCLImagingResult)
        {
            if (aPatientPCLImagingResult == null
            || aPatientPCLImagingResult.PatientPCLRequest == null
            || aPatientPCLImagingResult.PatientPCLRequest.Patient == null)
            {
                return;
            }
            Accession_Number = aPatientPCLImagingResult.PatientPCLRequest.PCLRequestNumID;
            Patient_ID = aPatientPCLImagingResult.PatientPCLRequest.Patient.PatientCode;
            Report_Date = aPatientPCLImagingResult.PerformedDate.ToString("yyyy-MM-dd HH:mm:ss");
            Report_Desc = aPatientPCLImagingResult.TemplateResultDescription;
            Report_Result = aPatientPCLImagingResult.TemplateResult;
            Reading_Physician_ID = (long)aPatientPCLImagingResult.PerformStaffID;
            Reading_Physician = aPatientPCLImagingResult.PerformStaffFullName;
            Status = aPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE ? "COMPLETED" : "";
        }

        public string Accession_Number { get; set; }
        public string Patient_ID { get; set; }
        public string Report_Date { get; set; }
        public string Report_Desc { get; set; }
        public string Report_Result { get; set; }
        public long Reading_Physician_ID { get; set; }
        public string Reading_Physician { get; set; }
        // COMPLETED/READ/FINAL/PRINT
        public string Status { get; set; }
    }

    public partial class LinkViewerFromPAC
    {
        public LinkViewerFromPAC(string InputAccessionNumber, string InputAlternatePatientID)
        {
            accessionNumber = InputAccessionNumber;
            alternatePatientID = InputAlternatePatientID;
        }

        public string accessionNumber { get; set; }
        public string alternatePatientID { get; set; }
    }

    public partial class PCLObjectFromHISToPACService
    {
        private string DateFormatStr = "yyyy-MM-dd HH:mm:ss";
        public PCLObjectFromHISToPACService(PatientPCLRequest aPatientPCLRequest, PatientPCLImagingResult patientPCLImagingResult = null)
        {
            if (aPatientPCLRequest.Patient == null
            || aPatientPCLRequest.PatientPCLRequestIndicators == null
            || aPatientPCLRequest.PatientPCLRequestIndicators.Count != 1
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType == null)
            || (aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null)
            || (!aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.IsSendToPAC))
            {
                return;
            }
            patientPCLReqID = aPatientPCLRequest.PatientPCLReqID.ToString();
            pCLRequestNumID = aPatientPCLRequest.PCLRequestNumID.ToString();
            createdDate = aPatientPCLRequest.CreatedDate.ToString(DateFormatStr);
            patientCode = aPatientPCLRequest.Patient.PatientCode;
            patientFullName = aPatientPCLRequest.Patient.FullName;
            patientDOB = ((DateTime)aPatientPCLRequest.Patient.DOB).ToString(DateFormatStr);
            patientGender = aPatientPCLRequest.Patient.Gender;
            patientFullStreetAddress = aPatientPCLRequest.Patient.PatientFullStreetAddress;
            patientCellPhoneNumber = aPatientPCLRequest.Patient.PatientCellPhoneNumber;
            deptName = aPatientPCLRequest.RequestedDepartment == null ? aPatientPCLRequest.ReqFromDeptLocIDName : aPatientPCLRequest.RequestedDepartment.DeptName;
            requestedDoctorPrefix = aPatientPCLRequest.RequestedDoctorPrefix ?? "";
            requestedDoctorCode = aPatientPCLRequest.RequestedDoctorCode ?? "";
            requestedDoctorName = aPatientPCLRequest.RequestedDoctorName ?? aPatientPCLRequest.StaffIDName ?? "";
            pCLExamTypeCode = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeCode;
            pCLExamTypeName = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.PCLExamTypeName;
            diagnosis = aPatientPCLRequest.Diagnosis;
            v_PCLRequestStatus = ((long)aPatientPCLRequest.V_PCLRequestStatus).ToString();
            subCategoryCodeToPAC = aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID == null ? "" : aPatientPCLRequest.PatientPCLRequestIndicators.First().PCLExamType.ObjPCLExamTypeSubCategoryID.SubCategoryCodeToPAC;
            if (patientPCLImagingResult != null)
            {
                performedDate = (patientPCLImagingResult.PerformedDate).ToString(DateFormatStr);
                performStaffID = patientPCLImagingResult.PerformStaffID.ToString();
                performStaffFullName = patientPCLImagingResult.PerformStaffFullName;
                performStaffCode = patientPCLImagingResult.PerformStaffCode;
                performStaffPrefix = patientPCLImagingResult.PerformStaffPrefix;
                resultStaffCode = patientPCLImagingResult.ResultStaffCode;
                resultStaffFullName = patientPCLImagingResult.ResultStaffFullName;
                resultStaffPrefix = patientPCLImagingResult.ResultStaffPrefix;
                templateResultDescription = patientPCLImagingResult.TemplateResultDescription;
                templateResult = patientPCLImagingResult.TemplateResult;
            }

        }

        public PCLObjectFromHISToPACService(PatientPCLImagingResult patientPCLImagingResult)
        {
            if (patientPCLImagingResult != null)
            {
                PatientPCLRequest patientPCLRequest = patientPCLImagingResult.PatientPCLRequest;
                patientPCLReqID = patientPCLRequest.PatientPCLReqID.ToString();
                pCLRequestNumID = patientPCLRequest.PCLRequestNumID.ToString();
                createdDate = patientPCLRequest.CreatedDate.ToString(DateFormatStr);
                patientCode = patientPCLRequest.Patient.PatientCode;
                patientFullName = patientPCLRequest.Patient.FullName;
                patientDOB = ((DateTime)patientPCLRequest.Patient.DOB).ToString(DateFormatStr);
                patientGender = patientPCLRequest.Patient.Gender;
                patientFullStreetAddress = patientPCLRequest.Patient.PatientFullStreetAddress;
                patientCellPhoneNumber = patientPCLRequest.Patient.PatientCellPhoneNumber;
                deptName = patientPCLRequest.RequestedDepartment == null ? patientPCLRequest.ReqFromDeptLocIDName : patientPCLRequest.RequestedDepartment.DeptName;
                requestedDoctorPrefix = patientPCLRequest.RequestedDoctorPrefix ?? "";
                requestedDoctorCode = patientPCLRequest.RequestedDoctorCode ?? "";
                requestedDoctorName = patientPCLRequest.RequestedDoctorName ?? patientPCLRequest.StaffIDName ?? "";                
                diagnosis = patientPCLRequest.Diagnosis;
                v_PCLRequestStatus = ((long)patientPCLRequest.V_PCLRequestStatus).ToString();
                performedDate = (patientPCLImagingResult.PerformedDate).ToString(DateFormatStr);
                performStaffID = patientPCLImagingResult.PerformStaffID.ToString();
                performStaffFullName = patientPCLImagingResult.PerformStaffFullName;
                performStaffCode = patientPCLImagingResult.PerformStaffCode;
                performStaffPrefix = patientPCLImagingResult.PerformStaffPrefix;
                resultStaffCode = patientPCLImagingResult.ResultStaffCode;
                resultStaffFullName = patientPCLImagingResult.ResultStaffFullName;
                resultStaffPrefix = patientPCLImagingResult.ResultStaffPrefix;
                templateResultDescription = patientPCLImagingResult.TemplateResultDescription;
                templateResult = patientPCLImagingResult.TemplateResult;
                if (patientPCLRequest.PCLExamTypeItem != null)
                {
                    PCLExamType temp = patientPCLRequest.PCLExamTypeItem;
                    pCLExamTypeCode = temp.PCLExamTypeCode;
                    pCLExamTypeName = temp.PCLExamTypeName;
                    subCategoryCodeToPAC = temp.ObjPCLExamTypeSubCategoryID == null ? "" : temp.ObjPCLExamTypeSubCategoryID.SubCategoryCodeToPAC;
                }
                if (patientPCLImagingResult.PatientPCLImagingResultDetail != null && patientPCLImagingResult.PatientPCLImagingResultDetail.Count() > 0
                    && string.IsNullOrEmpty(templateResultDescription))
                {
                    string resultDescription = "";
                    foreach (var itemResultDetail in patientPCLImagingResult.PatientPCLImagingResultDetail.OrderBy(x => x.PrintIdx))
                    {
                        if (!itemResultDetail.IsTechnique)
                        {
                            if (itemResultDetail.IsBold)
                            {
                                resultDescription = resultDescription.Replace(Environment.NewLine, "</br>") + itemResultDetail.PCLExamTestItemName + "</br>";
                            }
                            else
                            {
                                resultDescription = resultDescription + itemResultDetail.Value.Replace(Environment.NewLine, "</br>") + "</br>";
                            }
                        }
                    }
                    templateResultDescription = resultDescription;
                }
            }

        }

        public string patientPCLReqID { get; set; }
        public string pCLRequestNumID { get; set; }
        //  2020-10-07 16:08:47.373
        public string createdDate { get; set; }
        public string patientCode { get; set; }
        // 1991-01-01
        public string patientDOB { get; set; }
        // M: Male F: Female
        public string patientFullName { get; set; }
        public string patientGender { get; set; }
        public string patientFullStreetAddress { get; set; }
        public string patientCellPhoneNumber { get; set; }
        public string deptName { get; set; }
        public string requestedDoctorPrefix { get; set; }
        public string requestedDoctorCode { get; set; }
        public string requestedDoctorName { get; set; }
        public string pCLExamTypeCode { get; set; }
        public string pCLExamTypeName { get; set; }
        public string diagnosis { get; set; }
        public string objPCLExamTypeSubCategoryID { get; set; }
        public string v_PCLRequestStatus { get; set; }
        public string subCategoryCodeToPAC { get; set; }
        public string performedDate { get; set; }
        public string performStaffID { get; set; }
        public string performStaffFullName { get; set; }
        public string performStaffCode { get; set; }
        public string performStaffPrefix { get; set; }
        public string resultStaffCode { get; set; }
        public string resultStaffFullName { get; set; }
        public string resultStaffPrefix { get; set; }
        public string templateResultDescription { get; set; }
        public string templateResult { get; set; }
    }
}
