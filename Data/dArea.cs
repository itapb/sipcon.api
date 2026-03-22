using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
using Util;

namespace Data
{
  public class dArea
  {
    // test data
    private readonly SemaphoreSlim _semaphore;

    public dArea() {
      Util.Setting.GetSettings(true);
      _semaphore = new SemaphoreSlim(100, 150);
    }

    public async Task<Response<List<Models.Area>>> GetAll()
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _GetAll();
      }
      finally
      {
        _semaphore.Release();
      }
    }

    public async Task<Result> Post_Areas(List<Area> _list)
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _Post_Areas(_list);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    public async Task<Response<Models.Area>> GetOne(Int32 AreaId)
    {
      await _semaphore.WaitAsync(Util.Setting.TimeOut);
      try
      {
        return await _GetOne(AreaId);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private async Task<Response<List<Models.Area>>> _GetAll()
    {
      Response<List<Models.Area>> _response = new Response<List<Models.Area>>();
      try
      {
        Mapping _mapping = new Mapping();
        _mapping.AddItem("Id", "ID");
        _mapping.AddItem("Name", "VNAME");
        _mapping.AddItem("IsActive", "BACTIVE");

        Util.Data _data = Util.Data.GetInstance();
        DataTable _table = await _data.GetDataTable("USP_GET_AREAS");
        _response.Data = _data.GetList<Models.Area>(_mapping, _table);
        _response.SetGetResponse(_table);
      }
      catch (Exception ex)
      {
        _response.SetError(ex);
      }

      return _response;
    }

    private async Task<Response<Models.Area>> _GetOne(Int32 AreaId)
    {
      Response<Models.Area> _response = new Response<Models.Area>();
      try
      {
        Util.Parameter _parameter = new Util.Parameter();
        _parameter.AddSqlParameter("@ID", AreaId);

        Mapping _mapping = new Mapping();
        _mapping.AddItem("Id", "ID");
        _mapping.AddItem("Name", "VNAME");
        _mapping.AddItem("IsActive", "BACTIVE");

        Util.Data _data = Util.Data.GetInstance();
        DataTable _table = await _data.GetDataTable("USP_GET_AREA_BY_ID", _parameter);
        _response.Data = _data.GetItem<Models.Area>(_mapping, _table);
        _response.SetGetResponse(_table);
      }
      catch (Exception ex)
      {
        _response.SetError(ex);
      }

      return _response;
    }


    private async Task<Result> _Post_Areas(List<Area> _list)
    {
      List<Result> _results = new List<Result>();
      try
      {
        string _jsonstring = Util.Json.ConvertToJsonString(_list);

        Util.Parameter _parameter = new Util.Parameter();
        _parameter.AddSqlParameter("@DATA", _jsonstring);

        Mapping _mapping = new Mapping();
        _mapping.SetDefaultPostMapping();

        Util.Data _data = Util.Data.GetInstance();
        _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_AREA", _mapping, _parameter);
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
