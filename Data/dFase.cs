using Models;
using System.Data;
using Util;

namespace Data
{
  public class dFase
  {
    private readonly SemaphoreSlim _semaphore;

    public dFase() {
      Util.Setting.GetSettings(true);
      _semaphore = new SemaphoreSlim(100, 150);
    }

    public async Task<Response<List<Models.Fase>>> GetAll(Int32? AreaId, Int32? SupplierId, Int32? DealerId, bool IsActive)
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _GetAll(AreaId, SupplierId, DealerId, IsActive);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    public async Task<Response<Models.Fase>> GetOne(Int32 FaseId)
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _GetOne(FaseId);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    public async Task<Result> Post_Fases(List<Fase> _list, Int32 userId)
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _Post_Fases(_list, userId);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private async Task<Response<List<Models.Fase>>> _GetAll(Int32? AreaId, Int32? SupplierId, Int32? DealerId, bool IsActive)
    {
      Response<List<Models.Fase>> _response = new Response<List<Models.Fase>>();

      try
      {
        Util.Parameter _parameter = new Util.Parameter();
        _parameter.AddSqlParameter("@IDAREA", AreaId);
        _parameter.AddSqlParameter("@IDSUPPLIER", SupplierId);
        _parameter.AddSqlParameter("@IDDEALER", DealerId);
        _parameter.AddSqlParameter("@BACTIVE", IsActive);

        Mapping _mapping = new Mapping();
        _mapping.AddItem("Id", "ID");
        _mapping.AddItem("Name", "VNAME");
        _mapping.AddItem("OrderBy", "IORDERBY");
        _mapping.AddItem("IsActive", "BACTIVE");
        _mapping.AddItem("AreaId", "IDAREA");
        _mapping.AddItem("Area", "AREA");
        _mapping.AddItem("DealerId", "IDDEALER");
        _mapping.AddItem("Dealer", "DEALER");
        _mapping.AddItem("SupplierId", "IDSUPPLIER");
        _mapping.AddItem("Supplier", "SUPPLIER");
        _mapping.AddItem("UpdatedBy", "VUPDATEDBY");

        Util.Data _data = Util.Data.GetInstance();
        DataTable _table = await _data.GetDataTable("USP_GET_FASES", _parameter);
        _response.Data = _data.GetList<Models.Fase>(_mapping, _table);
        _response.SetGetResponse(_table);
      }
      catch (Exception ex)
      {
        _response.SetError(ex);
      }

      return _response;
    }

    private async Task<Response<Models.Fase>> _GetOne(Int32 FaseId)
    {
      Response<Models.Fase> _response = new Response<Models.Fase>();
      try
      {
        Util.Parameter _parameter = new Util.Parameter();
        _parameter.AddSqlParameter("@ID", FaseId);

        Mapping _mapping = new Mapping();
        _mapping.AddItem("Id", "ID");
        _mapping.AddItem("Name", "VNAME");
        _mapping.AddItem("OrderBy", "IORDERBY");
        _mapping.AddItem("IsActive", "BACTIVE");
        _mapping.AddItem("AreaId", "IDAREA");
        _mapping.AddItem("Area", "AREA");
        _mapping.AddItem("DealerId", "IDDEALER");
        _mapping.AddItem("Dealer", "DEALER");
        _mapping.AddItem("SupplierId", "IDSUPPLIER");
        _mapping.AddItem("Supplier", "SUPPLIER");
        _mapping.AddItem("UpdatedBy", "VUPDATEDBY");


        Util.Data _data = Util.Data.GetInstance();
        DataTable _table = await _data.GetDataTable("USP_GET_FASE_BY_ID", _parameter);
        _response.Data = _data.GetItem<Models.Fase>(_mapping, _table);
        _response.SetGetResponse(_table);
      }
      catch (Exception ex)
      {
        _response.SetError(ex);
      }

      return _response;
    }

    private async Task<Result> _Post_Fases(List<Fase> _list, Int32 userId)
    {
      List<Result> _results = new List<Result>();
      try
      {
        string _jsonstring = Util.Json.ConvertToJsonString(_list);

        Util.Parameter _parameter = new Util.Parameter();
        _parameter.AddSqlParameter("@DATA", _jsonstring);
        _parameter.AddSqlParameter("@IDUSER", userId);

        Mapping _mapping = new Mapping();
        _mapping.SetDefaultPostMapping();

        Util.Data _data = Util.Data.GetInstance();
        _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_FASE", _mapping, _parameter);
      }
      catch (Exception ex)
      {
        Util.Log.Error(ex);
        throw;
      }

      return _results[0];
    }
  }
}
