﻿using GenshinPray.Attribute;
using GenshinPray.Common;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Models.VO;
using GenshinPray.Service;
using GenshinPray.Type;
using GenshinPray.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinPray.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrayInfoController : BaseController
    {
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected PrayRecordService prayRecordService;
        protected MemberGoodsService memberGoodsService;

        public PrayInfoController()
        {
            this.authorizeService = new AuthorizeService();
            this.memberService = new MemberService();
            this.goodsService = new GoodsService();
            this.prayRecordService = new PrayRecordService();
            this.memberGoodsService = new MemberGoodsService();
        }

        /// <summary>
        /// 获取当前所有祈愿池的up内容
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult GetPondInfo()
        {
            try
            {
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                Dictionary<int, YSUpItem> armUpItemDic = goodsService.GetUpItem(authorizePO.Id, YSPondType.武器);
                Dictionary<int, YSUpItem> roleUpItemDic = goodsService.GetUpItem(authorizePO.Id, YSPondType.角色);
                Dictionary<int, YSUpItem> permUpItemDic = DataCache.DefaultUpItem[YSPondType.常驻];
                return ApiResult.Success(new
                {
                    arm = armUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    role = roleUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    perm = permUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取成员祈愿详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult GetMemberPrayDetail(string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                MemberGoodsCountDTO memberGoodsCount = memberGoodsService.GetMemberGoodsCount(authorizePO.Id, memberCode);
                return ApiResult.Success(new
                {
                    Role180Surplus = memberInfo.Role180Surplus,
                    Role90Surplus = memberInfo.Role90Surplus,
                    Role10Surplus = memberInfo.Role10Surplus,
                    ArmAssignValue = memberInfo.ArmAssignValue,
                    Arm80Surplus = memberInfo.Arm80Surplus,
                    Arm10Surplus = memberInfo.Arm10Surplus,
                    Perm90Surplus = memberInfo.Perm90Surplus,
                    Perm10Surplus = memberInfo.Perm10Surplus,
                    RolePrayTimes = memberInfo.RolePrayTimes,
                    ArmPrayTimes = memberInfo.ArmPrayTimes,
                    PermPrayTimes = memberInfo.PermPrayTimes,
                    TotalPrayTimes = memberInfo.TotalPrayTimes,
                    Star4Count = memberGoodsCount.Star4Count,
                    Star5Count = memberGoodsCount.Star5Count,
                    RoleStar4Count = memberGoodsCount.RoleStar4Count,
                    ArmStar4Count = memberGoodsCount.ArmStar4Count,
                    PermStar4Count = memberGoodsCount.PermStar4Count,
                    RoleStar5Count = memberGoodsCount.RoleStar5Count,
                    ArmStar5Count = memberGoodsCount.ArmStar5Count,
                    PermStar5Count = memberGoodsCount.PermStar5Count,
                    Star4Rate = NumberHelper.GetRate(memberGoodsCount.Star4Count, memberInfo.TotalPrayTimes),
                    Star5Rate = NumberHelper.GetRate(memberGoodsCount.Star5Count, memberInfo.TotalPrayTimes),
                    RoleStar4Rate = NumberHelper.GetRate(memberGoodsCount.RoleStar4Count, memberInfo.RolePrayTimes),
                    ArmStar4Rate = NumberHelper.GetRate(memberGoodsCount.ArmStar4Count, memberInfo.ArmPrayTimes),
                    PermStar4Rate = NumberHelper.GetRate(memberGoodsCount.PermStar4Count, memberInfo.PermPrayTimes),
                    RoleStar5Rate = NumberHelper.GetRate(memberGoodsCount.RoleStar5Count, memberInfo.RolePrayTimes),
                    ArmStar5Rate = NumberHelper.GetRate(memberGoodsCount.ArmStar5Count, memberInfo.ArmPrayTimes),
                    PermStar5Rate = NumberHelper.GetRate(memberGoodsCount.PermStar5Count, memberInfo.PermPrayTimes)
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取群内欧气排行
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult GetLuckRanking()
        {
            try
            {
                int top = 20;
                int days = 15;
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                LuckRankingVO luckRankingVO = memberGoodsService.getLuckRanking(authorizePO.Id, days, top);
                return ApiResult.Success(luckRankingVO);
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 定轨武器
        /// </summary>
        /// <param name="memberCode"></param>
        /// <param name="goodsName"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCode]
        public ApiResult SetMemberAssign(string memberCode, string goodsName)
        {
            try
            {
                int pondIndex = 0;
                checkNullParam(memberCode, goodsName);
                GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName);
                if (goodsInfo == null) return ApiResult.GoodsNotFound;
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);

                Dictionary<int, YSUpItem> upItemDic = goodsService.GetUpItem(authorizePO.Id, YSPondType.武器);
                YSUpItem ySUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ySUpItem == null) return ApiResult.PondNotConfigured;
                if (ySUpItem.Star5UpList.Where(o => o.GoodsID == goodsInfo.Id).Any() == false) return ApiResult.AssignNotFound;
                MemberPO memberInfo = memberService.SetArmAssign(goodsInfo, authorizePO.Id, memberCode);
                return ApiResult.Success();
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 获取成员定轨信息
        /// </summary>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode]
        public ApiResult GetMemberAssign(string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null || memberInfo.ArmAssignId == 0) return ApiResult.Success("未找到定轨信息");
                GoodsPO goodsInfo = goodsService.GetGoodsById(memberInfo.ArmAssignId);
                if (goodsInfo == null) return ApiResult.Success("未找到定轨信息");
                return ApiResult.Success(new
                {
                    GoodsName = goodsInfo.GoodsName,
                    GoodsType = Enum.GetName(typeof(YSGoodsType), goodsInfo.GoodsType),
                    GoodsSubType = Enum.GetName(typeof(YSGoodsSubType), goodsInfo.GoodsSubType),
                    RareType = Enum.GetName(typeof(YSRareType), goodsInfo.RareType),
                    AssignValue = memberInfo.ArmAssignValue
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }

        [HttpGet]
        [AuthCode]
        public ApiResult GetMemberPrayRecords(string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                var authorzation = HttpContext.Request.Headers["authorzation"];
                AuthorizePO authorizePO = authorizeService.GetAuthorize(authorzation);
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                List<PrayRecordDTO> allStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, 20);
                List<PrayRecordDTO> armStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.武器, 20);
                List<PrayRecordDTO> roleStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.角色, 20);
                List<PrayRecordDTO> permStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.常驻, 20);
                List<PrayRecordDTO> allStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, 20);
                List<PrayRecordDTO> armStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.武器, 20);
                List<PrayRecordDTO> roleStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.角色, 20);
                List<PrayRecordDTO> permStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.常驻, 20);
                return ApiResult.Success(new
                {
                    star5 = new
                    {
                        arm = memberGoodsService.ChangeToPrayRecordVO(armStar5List),
                        role = memberGoodsService.ChangeToPrayRecordVO(roleStar5List),
                        perm = memberGoodsService.ChangeToPrayRecordVO(permStar5List),
                        all = memberGoodsService.ChangeToPrayRecordVO(allStar5List)
                    },
                    star4 = new
                    {
                        arm = memberGoodsService.ChangeToPrayRecordVO(armStar4List),
                        role = memberGoodsService.ChangeToPrayRecordVO(roleStar4List),
                        perm = memberGoodsService.ChangeToPrayRecordVO(permStar4List),
                        all = memberGoodsService.ChangeToPrayRecordVO(allStar4List)
                    }
                });
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }


    }
}
