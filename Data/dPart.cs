using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;
using Util;

namespace Data
{
    public class dPart
    {
        private readonly SemaphoreSlim _semaphore;
  

        public dPart()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, rowfrom, filter, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetAll(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter,Int32? partId = null)
        {
            Response _response = (partId == null) ? new Response(true) : new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", partId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("InnerCode", "VINNERCODE");
                _mapping.AddItem("MasterCode", "VMASTERCODE");
                _mapping.AddItem("AlterCode", "VALTERCODE");
                _mapping.AddItem("ReplacementCode", "VREPLACEMENTCODE");
                _mapping.AddItem("BarCode", "VBARCODE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("FamilyId", "IDFAMILY");
                _mapping.AddItem("SubFamilyId", "IDSUBFAMILY");
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("Cost", "NCOST");
                _mapping.AddItem("Discount", "NDISCOUNT");
                _mapping.AddItem("Weight", "NWEIGHT");
                _mapping.AddItem("Size", "VSIZE");
                _mapping.AddItem("MinSale", "IMINSALE");
                _mapping.AddItem("Packing", "IPACKING");
                _mapping.AddItem("Rating", "VRATING");
                _mapping.AddItem("Sell", "BSELL");
                _mapping.AddItem("Purchase", "BPURCHASE");
                _mapping.AddItem("Warranty", "BWARRANTY");
                _mapping.AddItem("License", "BLICENSE");
                _mapping.AddItem("Original", "BORIGINAL");
                _mapping.AddItem("Serializable", "BSERIALIZABLE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("UmId", "IDUM");
                _mapping.AddItem("TaxId", "IDTAX");
                _mapping.AddItem("TypeName", "VPARTTYPE");
                _mapping.AddItem("FamilyName", "VFAMILY");
                _mapping.AddItem("SubFamilyName", "VSUBFAMILY");
                _mapping.AddItem("TaxName", "VTAX");
                _mapping.AddItem("UmName", "VUM");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("SupplierReference", "VREFERENCE"); 
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("Available", "IAVAILABLE");
                _mapping.AddItem("AlterDescription", "VALTERDESCRIPTION2");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PARTS", _parameter);
                _response.Data = (partId == null) ? _data.GetList<Models.Part>(_mapping, _table) : _data.GetItem<Models.Part>(_mapping, _table);
                _response.SetGetResponse(_table);
              


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetModels(Int32 partId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetModels(partId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetModels( Int32 partId)
        {
            Response _response = new Response(true);
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPART", partId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("IsRelated", "BRELATED");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
            

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PARTMODEL", _parameter);
                _response.Data = _data.GetList<Models.RelatedModel>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> GetOne( Int32 userId, Int32 partId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId,null,null,null, partId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<Response> GetByModel(Int32 modelId,Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetByModel(modelId, userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response> _GetByModel(Int32 modelId,Int32 userId, Int32? supplierId)
        {
            Response _response = new Response(true);
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDMODEL", modelId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("InnerCode", "VINNERCODE");
                _mapping.AddItem("MasterCode", "VMASTERCODE");
                _mapping.AddItem("AlterCode", "VALTERCODE");
                _mapping.AddItem("ReplacementCode", "VREPLACEMENTCODE");
                _mapping.AddItem("BarCode", "VBARCODE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("FamilyId", "IDFAMILY");
                _mapping.AddItem("SubFamilyId", "IDSUBFAMILY");
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("Cost", "NCOST");
                _mapping.AddItem("Discount", "NDISCOUNT");
                _mapping.AddItem("Weight", "NWEIGHT");
                _mapping.AddItem("Size", "VSIZE");
                _mapping.AddItem("MinSale", "IMINSALE");
                _mapping.AddItem("Packing", "IPACKING");
                _mapping.AddItem("Rating", "VRATING");
                _mapping.AddItem("Sell", "BSELL");
                _mapping.AddItem("Purchase", "BPURCHASE");
                _mapping.AddItem("Warranty", "BWARRANTY");
                _mapping.AddItem("License", "BLICENSE");
                _mapping.AddItem("Original", "BORIGINAL");
                _mapping.AddItem("Serializable", "BSERIALIZABLE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("UmId", "IDUM");
                _mapping.AddItem("TaxId", "IDTAX");
                _mapping.AddItem("TypeName", "VPARTTYPE");
                _mapping.AddItem("FamilyName", "VFAMILY");
                _mapping.AddItem("SubFamilyName", "VSUBFAMILY");
                _mapping.AddItem("TaxName", "VTAX");
                _mapping.AddItem("UmName", "VUM");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("SupplierReference", "VREFERENCE");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PARTSBYMODEL", _parameter);
                _response.Data = _data.GetList<Models.Part>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        private async Task<Response> _GetUm()
        {
            Response _response = new Response(true);
            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_UM");
                _response.Data = _data.GetList<Models.Um>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetUm()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetUm();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetFamily()
        {
            Response _response = new Response(true);
            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("PartTypeId", "IDTIPO");
                _mapping.AddItem("PartTypeName", "VPARTNAME");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FAMILY");
                _response.Data = _data.GetList<Models.Family>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetFamily()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFamily();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetSubFamily()
        {
            Response _response = new Response(true);

            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("FamilyId", "IDFAMILY");
                _mapping.AddItem("FamilyName", "VFAMILY");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SUBFAMILY");
                _response.Data = _data.GetList<Models.SubFamily>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response ;
        }

        public async Task<Response> GetSubFamily()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetSubFamily();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetTaxes()
        {
            Response _response = new Response(true);

            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Amount", "NAMOUNT");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_TAX");
                _response.Data = _data.GetList<Models.Tax>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetTaxes()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetTaxes();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _GetPartType()
        {
            Response _response = new Response(true);

            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");


          
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PARTTYPES");
                _response.Data = _data.GetList<Models.PartType>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response ;
        }

        public async Task<Response> GetPartType()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPartType();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<Part>> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                Response _response = new Response();
                _response = await _GetAll(userId, supplierId, null,filter);
                return (List<Part>)_response.Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response> Post_Parts(List<Models.Part> _list, List<Models.RelatedModel> _models, Int32 userId, bool bFull = false)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Parts(_list, _models, userId, bFull);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Parts(List<Models.Part> _list, List<Models.RelatedModel> _models, Int32 userId, bool bFull = false )
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);
                string _jsonstring2 = Util.Json.ConvertToJsonString(_models);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@MODELS", _jsonstring2);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BFULL", bFull);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PARTS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> Post_Actions(List<Models.Action> _list,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Actions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response> _Post_Actions(List<Models.Action> _list,Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PARTS_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

    }
}
