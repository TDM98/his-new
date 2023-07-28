namespace aEMR.ViewContracts
{
    public interface IObstetricGynecologicalHistory
    {
        long PatientID { set; get; }

        long PtRegDetailID { set; get; }

        bool IsMedicalExamination { set; get; }
}
}