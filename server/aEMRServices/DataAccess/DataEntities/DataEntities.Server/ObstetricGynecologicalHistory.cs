using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class ObstetricGynecologicalHistory : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long ObstetricGynecologicalHistoryID
        {
            get
            {
                return _ObstetricGynecologicalHistoryID;
            }
            set
            {
                if (_ObstetricGynecologicalHistoryID == value)
                {
                    return;
                }
                _ObstetricGynecologicalHistoryID = value;
                RaisePropertyChanged("ObstetricGynecologicalHistoryID");
            }
        }
        private long _ObstetricGynecologicalHistoryID;

        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PatientID;

        [DataMemberAttribute()]
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                if (_PtRegDetailID == value)
                {
                    return;
                }
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }
        private long _PtRegDetailID;

        [DataMemberAttribute()]
        public int Menarche
        {
            get
            {
                return _Menarche;
            }
            set
            {
                if (_Menarche == value)
                {
                    return;
                }
                _Menarche = value;
                RaisePropertyChanged("Menarche");
            }
        }
        private int _Menarche;

        [DataMemberAttribute()]
        public bool MenstruationIsRegular
        {
            get
            {
                return _MenstruationIsRegular;
            }
            set
            {
                if (_MenstruationIsRegular == value)
                {
                    return;
                }
                _MenstruationIsRegular = value;
                RaisePropertyChanged("MenstruationIsRegular");
            }
        }
        private bool _MenstruationIsRegular;

        [DataMemberAttribute()]
        public int MenstrualCycle
        {
            get
            {
                return _MenstrualCycle;
            }
            set
            {
                if (_MenstrualCycle == value)
                {
                    return;
                }
                _MenstrualCycle = value;
                RaisePropertyChanged("MenstrualCycle");
            }
        }
        private int _MenstrualCycle;

        [DataMemberAttribute()]
        public int MenstrualVolume
        {
            get
            {
                return _MenstrualVolume;
            }
            set
            {
                if (_MenstrualVolume == value)
                {
                    return;
                }
                _MenstrualVolume = value;
                RaisePropertyChanged("MenstrualVolume");
            }
        }
        private int _MenstrualVolume;

        [DataMemberAttribute()]
        public bool Dysmenorrhea
        {
            get
            {
                return _Dysmenorrhea;
            }
            set
            {
                if (_Dysmenorrhea == value)
                {
                    return;
                }
                _Dysmenorrhea = value;
                RaisePropertyChanged("Dysmenorrhea");
            }
        }
        private bool _Dysmenorrhea;

        [DataMemberAttribute()]
        public bool Married
        {
            get
            {
                return _Married;
            }
            set
            {
                if (_Married == value)
                {
                    return;
                }
                _Married = value;
                RaisePropertyChanged("Married");
            }
        }
        private bool _Married;

        [DataMemberAttribute()]
        public int Para
        {
            get
            {
                return _Para;
            }
            set
            {
                if (_Para == value)
                {
                    return;
                }
                _Para = value;
                RaisePropertyChanged("Para");
            }
        }
        private int _Para;

        [DataMemberAttribute()]
        public bool HasOBGYNSurgeries
        {
            get
            {
                return _HasOBGYNSurgeries;
            }
            set
            {
                if (_HasOBGYNSurgeries == value)
                {
                    return;
                }
                _HasOBGYNSurgeries = value;
                RaisePropertyChanged("HasOBGYNSurgeries");
            }
        }
        private bool _HasOBGYNSurgeries;

        [DataMemberAttribute()]
        public int NumberOfOBGYNSurgeries
        {
            get
            {
                return _NumberOfOBGYNSurgeries;
            }
            set
            {
                if (_NumberOfOBGYNSurgeries == value)
                {
                    return;
                }
                _NumberOfOBGYNSurgeries = value;
                RaisePropertyChanged("NumberOfOBGYNSurgeries");
            }
        }
        private int _NumberOfOBGYNSurgeries;

        [DataMemberAttribute()]
        public string NoteOBGYNSurgeries
        {
            get
            {
                return _NoteOBGYNSurgeries;
            }
            set
            {
                if (_NoteOBGYNSurgeries == value)
                {
                    return;
                }
                _NoteOBGYNSurgeries = value;
                RaisePropertyChanged("NoteOBGYNSurgeries");
            }
        }
        private string _NoteOBGYNSurgeries;

        [DataMemberAttribute()]
        public bool IsUseContraception
        {
            get
            {
                return _IsUseContraception;
            }
            set
            {
                if (_IsUseContraception == value)
                {
                    return;
                }
                _IsUseContraception = value;
                RaisePropertyChanged("IsUseContraception");
            }
        }
        private bool _IsUseContraception;

        [DataMemberAttribute()]
        public long V_Contraception
        {
            get
            {
                return _V_Contraception;
            }
            set
            {
                if (_V_Contraception == value)
                {
                    return;
                }
                _V_Contraception = value;
                RaisePropertyChanged("V_Contraception");
            }
        }
        private long _V_Contraception;

        [DataMemberAttribute()]
        public string V_ContraceptionStr
        {
            get
            {
                return _V_ContraceptionStr;
            }
            set
            {
                if (_V_ContraceptionStr == value)
                {
                    return;
                }
                _V_ContraceptionStr = value;
                RaisePropertyChanged("V_ContraceptionStr");
            }
        }
        private string _V_ContraceptionStr;

        [DataMemberAttribute()]
        public string NoteContraception
        {
            get
            {
                return _NoteContraception;
            }
            set
            {
                if (_NoteContraception == value)
                {
                    return;
                }
                _NoteContraception = value;
                RaisePropertyChanged("NoteContraception");
            }
        }
        private string _NoteContraception;

        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

        [DataMemberAttribute]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                if (_CreatedStaff == value)
                {
                    return;
                }
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public Staff LastUpdateStaff
        {
            get
            {
                return _LastUpdateStaff;
            }
            set
            {
                if (_LastUpdateStaff == value)
                {
                    return;
                }
                _LastUpdateStaff = value;
                RaisePropertyChanged("LastUpdateStaff");
            }
        }
        private Staff _LastUpdateStaff;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime _LastUpdateDate;
        #endregion
    }
}
