﻿using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISATQuaThucQuanCD_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanCDViewModel : Conductor<object>, ISATQuaThucQuanCD_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanCDViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            //Globals.EventAggregator.Subscribe(this);

            if (Globals.PatientPCLReqID_Imaging > 0)
            {
                PatientPCLReqID = Globals.PatientPCLReqID_Imaging;
                LoadInfo();
            }
        }


        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }

        #region properties

        private URP_FE_OesophagienneDiagnosis _curURP_FE_OesophagienneDiagnosis;

        public URP_FE_OesophagienneDiagnosis curURP_FE_OesophagienneDiagnosis
        {
            get { return _curURP_FE_OesophagienneDiagnosis; }
            set
            {
                if (_curURP_FE_OesophagienneDiagnosis == value)
                    return;
                _curURP_FE_OesophagienneDiagnosis = value;
                NotifyOfPropertyChange(() => curURP_FE_OesophagienneDiagnosis);
            }
        }

        #endregion

        #region method

        private void GetURP_FE_OesophagienneDiagnosisByID(long URP_FE_OesophagienneDiagnosisID
                                                          , long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosisID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item= contract.EndGetURP_FE_OesophagienneDiagnosis(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_OesophagienneDiagnosis= item;
                                                                                                      }
                                                                                                  }
                                                                                                  catch (Exception ex)
                                                                                                  {
                                                                                                      MessageBox.Show(ex.Message);
                                                                                                  }
                                                                                                  finally
                                                                                                  {
                                                                                                      Globals.IsBusy =
                                                                                                          false;
                                                                                                  }
                                                                                              }), null);
                                       }
                                   });

            t.Start();
        }

        #endregion

        public void LoadInfo()
        {
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
            curURP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
            GetURP_FE_OesophagienneDiagnosisByID(0, PatientPCLReqID);
          //  GetPCLExamResultTemplateListByTypeID(2, (int) AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);
        }

      
    }
}