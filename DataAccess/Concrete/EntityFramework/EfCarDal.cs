using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Concrete.EntityFramework
{

    public class EfCarDal : EfEntityRepositoryBase<Car, RentACarContext>, ICarDal
    {

        public List<CarDetailDto> GetCarDetails(Expression<Func<Car, bool>> filter = null)
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from p in filter == null ? context.Car : context.Car.Where(filter)
                             join c in context.Color
                             on p.ColorId equals c.ColorId
                             join d in context.Brand
                             on p.BrandId equals d.BrandId
                             select new CarDetailDto
                             {
                                 CarName = p.CarName,
                                 BrandName = d.BrandName,
                                 ColorName = c.ColorName,
                                 DailyPrice = p.DailyPrice,
                                 Description = p.Description,
                                 ModelYear = p.ModelYear,
                                 CarId = p.CarId,
                                 ImagePath = context.CarImage.Where(x => x.CarId == p.CarId).FirstOrDefault().ImagePath,
                                 Status = !context.Rental.Any(r => r.CarId == p.CarId && (r.ReturnDate == null || r.RentDate > DateTime.Now)),
                                 //CarFindex = p.CarFindex


                             };
                return result.ToList();
            }
        }
        public CarDetailDto GetCarDetail(Expression<Func<CarDetailDto, bool>> filter)
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from c in context.Car
                             join b in context.Brand on c.BrandId equals b.BrandId
                             join r in context.Color on c.ColorId equals r.ColorId
                             let i = context.CarImage.Where(x => x.CarId == c.CarId).FirstOrDefault()
                             select new CarDetailDto()
                             {
                                 CarName = c.CarName,
                                 CarId = c.CarId,
                                 BrandName = b.BrandName,
                                 DailyPrice = c.DailyPrice,
                                 ColorName = r.ColorName,
                                 Description = c.Description,
                                 CarFindex = c.CarFindex

                             };
                return result.SingleOrDefault(filter);
            }
        }
    }
}