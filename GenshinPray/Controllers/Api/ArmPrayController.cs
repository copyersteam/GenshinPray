﻿using GenshinPray.Attribute;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Service.PrayService;
using GenshinPray.Type;
using GenshinPray.Util;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinPray.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ArmPrayController : BasePrayController<ArmPrayService>
    {
        /// <summary>
        /// 单抽武器祈愿池
        /// </summary>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult PrayOne(string memberCode, bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                int pondIndex = 0;
                int prayCount = 1;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                var authorzation = GetAuthCode();
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                int prayTimesToday = prayRecordService.GetPrayTimesToday(authorizePO.Id);
                if (prayTimesToday >= authorizePO.DailyCall) return ApiResult.ApiMaximum;
                Dictionary<int, YSUpItem> upItemDic = goodsService.GetUpItem(authorizePO.Id, YSPondType.武器);
                YSUpItem ySUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ySUpItem == null) return ApiResult.PondNotConfigured;

                DbScoped.SugarScope.BeginTran();
                MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode);
                YSGoodsItem assignGoodsItem = memberInfo.ArmAssignId == 0 || ySUpItem.Star5UpList.Where(o => o.GoodsID == memberInfo.ArmAssignId).Any() == false ? null : goodsService.GetGoodsItemById(memberInfo.ArmAssignId);
                List<MemberGoodsDTO> memberGoods = goodsService.GetMemberGoods(authorizePO.Id, memberCode);
                YSPrayResult ySPrayResult = basePrayService.GetPrayResult(memberInfo, ySUpItem, assignGoodsItem, memberGoods, prayCount, imgWidth);
                prayRecordService.AddPrayRecord(authorizePO.Id, memberCode, prayCount);//添加调用记录
                memberGoodsService.AddMemberGoods(ySPrayResult, memberGoods, YSPondType.武器, authorizePO.Id, memberCode);//添加成员出货记录
                DbScoped.SugarScope.CommitTran();

                ApiPrayResult prayResult = basePrayService.CreatePrayResult(ySUpItem, ySPrayResult, authorizePO, prayTimesToday, toBase64);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"authorzation：{GetAuthCode()}，memberCode：{memberCode}，toBase64：{toBase64}，imgWidth：{imgWidth}");
                DbScoped.SugarScope.RollbackTran();
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 十连武器祈愿池
        /// </summary>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult PrayTen(string memberCode, bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                int pondIndex = 0;
                int prayCount = 10;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                var authorzation = GetAuthCode();
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                int prayTimesToday = prayRecordService.GetPrayTimesToday(authorizePO.Id);
                if (prayTimesToday >= authorizePO.DailyCall) return ApiResult.ApiMaximum;
                Dictionary<int, YSUpItem> upItemDic = goodsService.GetUpItem(authorizePO.Id, YSPondType.武器);
                YSUpItem ySUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ySUpItem == null) return ApiResult.PondNotConfigured;

                DbScoped.SugarScope.BeginTran();
                MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode);
                YSGoodsItem assignGoodsItem = memberInfo.ArmAssignId == 0 || ySUpItem.Star5UpList.Where(o => o.GoodsID == memberInfo.ArmAssignId).Any() == false ? null : goodsService.GetGoodsItemById(memberInfo.ArmAssignId);
                List<MemberGoodsDTO> memberGoods = goodsService.GetMemberGoods(authorizePO.Id, memberCode);
                YSPrayResult ySPrayResult = basePrayService.GetPrayResult(memberInfo, ySUpItem, assignGoodsItem, memberGoods, prayCount, imgWidth);
                prayRecordService.AddPrayRecord(authorizePO.Id, memberCode, prayCount);//添加调用记录
                memberGoodsService.AddMemberGoods(ySPrayResult, memberGoods, YSPondType.武器, authorizePO.Id, memberCode);//添加成员出货记录
                DbScoped.SugarScope.CommitTran();

                ApiPrayResult prayResult = basePrayService.CreatePrayResult(ySUpItem, ySPrayResult, authorizePO, prayTimesToday, toBase64);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"authorzation：{GetAuthCode()}，memberCode：{memberCode}，toBase64：{toBase64}，imgWidth：{imgWidth}");
                DbScoped.SugarScope.RollbackTran();
                return ApiResult.ServerError;
            }
        }


    }
}
