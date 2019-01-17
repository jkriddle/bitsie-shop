using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api
{
    public static class AutoMapperServiceBootstrap
    {
        public static void Init()
        {
            // Initialize mapping. Do not copy null values over defaults.
            Mapper.CreateMap<UserListInputModel, UserFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<LogListInputModel, LogFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<OrderListInputModel, OrderFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<PayoutInputModel, OrderFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<ProductListInputModel, ProductFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<ProductInputModel, Product>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<InvoiceListInputModel, InvoiceFilter>().ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
        }
    }
}