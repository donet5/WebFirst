﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace SoEasyPlatform 
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected  IMapper mapper;
        protected Repository<Menu> MenuDb=> new Repository<Menu>();
        protected Repository<DBConnection> DBConnectionDb => new Repository<DBConnection>();

        /// <summary>
        /// 验证数据库逻辑是否符合要求
        /// </summary>
        /// <param name="primaryValue"></param>
        /// <returns></returns>
        protected JsonResult ValidateModel(object primaryValue = null)
        {
            JsonResult errorResult = null;
            var validateItem = this.HttpContext.Items?.FirstOrDefault(it => it.Key.ToString() == Pubconst.ITEMKEY);
            if (validateItem != null && validateItem.Value.Value is ValidateUnique)
            {
                var unItem = (validateItem.Value.Value as ValidateUnique);
                var queryable = MenuDb.AsQueryable().AS(unItem.TableName).Where(new List<IConditionalModel>() {
                    new ConditionalModel(){ FieldName=unItem.DbColumnName,FieldValue=unItem.Value +""}
                });
                if (unItem.PrimaryKey != null && primaryValue != null && primaryValue.ToString() != "0")
                {
                    queryable.Where(new List<IConditionalModel>() {
                    new ConditionalModel(){ FieldName=unItem.PrimaryKey , ConditionalType=ConditionalType.NoEqual,FieldValue=primaryValue +""}
                });
                }
                if (queryable.Any())
                {
                    errorResult = new JsonResult(new ApiResult<List<KeyValuePair<string, string>>>()
                    {
                        Data = new List<KeyValuePair<string, string>>() {
                            new KeyValuePair<string, string>(unItem.FieldName,unItem.Message)
                        },
                        IsSuccess = false,
                        IsKeyValuePair = true
                    });
                }
            }

            return errorResult;
        }
    }
}