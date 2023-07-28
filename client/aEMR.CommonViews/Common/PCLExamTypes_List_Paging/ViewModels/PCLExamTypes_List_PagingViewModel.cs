﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;

namespace aEMR.Common.PCLExamTypes_List_Paging.ViewModels
{
    [Export(typeof(IPCLExamTypes_List_Paging)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypes_List_PagingViewModel : Conductor<object>, IPCLExamTypes_List_Paging
       , IHandle<PCLItemsEvent_Save>
       , IHandle<PCLExamTypeLocations_XMLInsert_Save_Event<Boolean>>
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        [ImportingConstructor]
        public PCLExamTypes_List_PagingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventArg.Subscribe(this);
        }


        private bool _IsEnableV_PCLMainCategory = true;
        public bool IsEnableV_PCLMainCategory
        {
            get { return _IsEnableV_PCLMainCategory; }
            set
            {
                if (_IsEnableV_PCLMainCategory != value)
                {
                    _IsEnableV_PCLMainCategory = value;
                    NotifyOfPropertyChange(() => IsEnableV_PCLMainCategory);
                }
            }
        }

        private bool _IsChildWindow = false;
        public bool IsChildWindow
        {
            get { return _IsChildWindow; }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                    NotifyOfPropertyChange(() => IsChildWindow);
                }
            }
        }

        private Visibility _IsNotInPCLItemsVisibility;
        public Visibility IsNotInPCLItemsVisibility
        {
            get { return _IsNotInPCLItemsVisibility; }
            set
            {
                if (_IsNotInPCLItemsVisibility != value)
                {
                    _IsNotInPCLItemsVisibility = value;
                    NotifyOfPropertyChange(() => IsNotInPCLItemsVisibility);
                }
            }
        }


        public void FormLoad()
        {
            authorization();

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;

            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;

            LoadV_PCLMainCategory();

            SearchCriteria.V_PCLMainCategory = ObjV_PCLMainCategory_Selected.LookupID;
            SearchCriteria.PCLExamTypeName = "";

            ObjPCLExamTypes_List_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypes_List_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypes_List_Paging_OnRefresh);

        }

        public void LoadForConfigPCLExamTypesIntoPCLForm()
        {
            authorization();

            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;

            LoadV_PCLMainCategory();

            SearchCriteria.V_PCLMainCategory = ObjV_PCLMainCategory_Selected.LookupID;
            SearchCriteria.PCLExamTypeName = "";

            ObjPCLExamTypes_List_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypes_List_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypes_List_Paging_OnRefresh);

        }

        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0185_G1_DSLoai)});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
                                    ObjV_PCLMainCategory.Insert(0, firstItem);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }
        //Main


        //Sub
        private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }

        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0532_G1_DSNhom) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjV_PCLMainCategory_Selected.LookupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);
                                PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                                firstItem.PCLExamTypeSubCategoryID = -1;
                                firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);

                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        //Sub


        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        void ObjPCLExamTypes_List_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypes_List_Paging(ObjPCLExamTypes_List_Paging.PageIndex, ObjPCLExamTypes_List_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<DataEntities.PCLExamType> _ObjPCLExamTypes_List_Paging;
        public PagedSortableCollectionView<DataEntities.PCLExamType> ObjPCLExamTypes_List_Paging
        {
            get { return _ObjPCLExamTypes_List_Paging; }
            set
            {
                _ObjPCLExamTypes_List_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_List_Paging);
            }
        }

        private void PCLExamTypes_List_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            if (CheckClickHeaderNotValid() == false)
                return;

            ObjPCLExamTypes_List_Paging.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3014_G1_DSPCLExamType) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLExamType> allItems = null;
                            bool bOK = false;

                            try
                            {
                                allItems = client.EndPCLExamTypes_List_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypes_List_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypes_List_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private bool CheckClickHeaderNotValid()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
                return true;
            return false;
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bBtnSearch = true;
            //bBtnSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
            //                                   (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mView);

        }

        #region checking account

        private bool _bBtnSearch = true;
        public bool bBtnSearch
        {
            get
            {
                return _bBtnSearch;
            }
            set
            {
                if (_bBtnSearch == value)
                    return;
                _bBtnSearch = value;
            }
        }
        #endregion

        public void btFind()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
            {
                ObjPCLExamTypes_List_Paging.PageIndex = 0;
                PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
            }
            else//-1 Text yêu cầu chọn
            {
                MessageBox.Show(eHCMSResources.A0335_G1_Msg_InfoChonLoai, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
            }
        }

        public void cboV_PCLMainCategory_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                Lookup Objtmp = (selectItem as Lookup);

                if (Objtmp != null)
                {
                    SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                    if (Objtmp.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
                        ObjPCLExamTypes_List_Paging.PageIndex = 0;
                        PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                    }
                    else
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
                        PCLExamTypeSubCategory_ByV_PCLMainCategory();
                    }
                }
            }
        }


        public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if (Ctr == null)
                return;

            PCLExamTypeSubCategory Objtmp = Ctr.SelectedItemEx as PCLExamTypeSubCategory;

            if (Objtmp != null)
            {
                SearchCriteria.PCLExamTypeSubCategoryID = Objtmp.PCLExamTypeSubCategoryID;
                if (SearchCriteria.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    ObjPCLExamTypes_List_Paging.PageIndex = 0;
                    PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                }
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            Globals.EventAggregator.Publish(new DbClickSelectedObjectEvent<PCLExamType> { Result = eventArgs.Value as PCLExamType });
            if (IsChildWindow)
            {
                TryClose();
            }
        }

        public void dtgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] != null)
                {
                    Globals.EventAggregator.Publish(new SelectedObjectEvent<PCLExamType> { Result = e.AddedItems[0] as PCLExamType });
                }
            }
        }

        public void Handle(PCLItemsEvent_Save message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {

                    if (SearchCriteria == null)
                        SearchCriteria = new PCLExamTypeSearchCriteria();

                    SearchCriteria.IsNotInPCLExamTypeLocations = true;
                    SearchCriteria.IsNotInPCLItems = false;
                    IsNotInPCLItemsVisibility = Visibility.Collapsed;


                    ObjPCLExamTypes_List_Paging.PageIndex = 0;
                    PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                }
            }
        }

        public void Handle(PCLExamTypeLocations_XMLInsert_Save_Event<bool> message)
        {
            if (this.GetView() != null)
            {
                if (SearchCriteria == null)
                    SearchCriteria = new PCLExamTypeSearchCriteria();

                SearchCriteria.IsNotInPCLExamTypeLocations = false;
                SearchCriteria.IsNotInPCLItems = true;
                IsNotInPCLItemsVisibility = Visibility.Collapsed;

                ObjPCLExamTypes_List_Paging.PageIndex = 0;
                PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
            }
        }

        public void hplViewDeptLocation_Click(object selectedItem)
        {
            PCLExamType p = (selectedItem as PCLExamType);

            if (p != null && p.PCLExamTypeID > 0)
            {                
                Action<IListDeptLocation_ByPCLExamTypeID> onInitDlg = delegate (IListDeptLocation_ByPCLExamTypeID typeInfo)
                {
                    typeInfo.PCLExamTypeID = p.PCLExamTypeID;
                };

                GlobalsNAV.ShowDialog<IListDeptLocation_ByPCLExamTypeID>(onInitDlg);
                
            }

        }
    }
}


