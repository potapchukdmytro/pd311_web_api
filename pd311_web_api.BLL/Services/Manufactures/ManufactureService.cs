﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pd311_web_api.BLL.DTOs.Manufactures;
using pd311_web_api.BLL.Services.Image;
using pd311_web_api.DAL;
using pd311_web_api.DAL.Entities;

namespace pd311_web_api.BLL.Services.Manufactures
{
    public class ManufactureService : IManufactureService
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly AppDbContext _context;

        public ManufactureService(IMapper mapper, AppDbContext context, IImageService imageService)
        {
            _mapper = mapper;
            _context = context;
            _imageService = imageService;
        }

        public async Task<bool> CreateAsync(CreateManufactureDto dto)
        {
            var entity = _mapper.Map<Manufacture>(dto);
            string? imageName = null;

            if(dto.Image != null)
            {
                imageName = await _imageService.SaveImageAsync(dto.Image, Settings.ManufacturesImagesPath);
                if(imageName != null)
                {
                    imageName = Path.Combine(Settings.ManufacturesImagesPath, imageName);
                }
            }

            entity.Image = imageName;
            await _context.Manufactures.AddAsync(entity);
            var result = await _context.SaveChangesAsync();

            return result != 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _context.Manufactures.FirstOrDefaultAsync(m => m.Id == id);

            if(entity == null)
            {
                return false;
            }

            if(!string.IsNullOrEmpty(entity.Image))
            {
                _imageService.DeleteImage(entity.Image);
            }

            _context.Manufactures.Remove(entity);
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> UpdateAsync(UpdateManufactureDto dto)
        {
            var entity = await _context.Manufactures
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.Id);

            if (entity == null)
            {
                return false;
            }

            entity = _mapper.Map(dto, entity);

            if(dto.Image != null)
            {
                if (!string.IsNullOrEmpty(entity.Image))
                {
                    _imageService.DeleteImage(entity.Image);
                }

                var imageName = await _imageService.SaveImageAsync(dto.Image, Settings.ManufacturesImagesPath);
                if (imageName != null)
                {
                    imageName = Path.Combine(Settings.ManufacturesImagesPath, imageName);
                }
                entity.Image = imageName;
            }

            _context.Update(entity);
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }
    }
}
