﻿using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;
using ProductApp.Dto;
using ProductApp.Entities;
using ProductApp.Models;
using ProductApp.Repositories.Interfaces;
using ProductApp.Services.Interfaces;

namespace ProductApp.Controllers;

public class FoodController : Controller
{
    private readonly IFoodService _foodService;
    private readonly IFoodRepository _FoodRepository;
    private readonly ICategoryRepository _categoryRepository;

    public FoodController(
       
        IFoodService foodService, 
        IFoodRepository foodRepository,
        ICategoryRepository categoryRepository
        )
    {
        _foodService = foodService;
        _FoodRepository = foodRepository;
        _categoryRepository = categoryRepository;
    }

    public IActionResult Index(long? categoryId=null)
    {
        var foods = _FoodRepository.GetQueryable();
        var categories = _categoryRepository.GetCategories();

        if (categoryId.HasValue)
        {
            foods = foods.Where(x => x.CategoryId == categoryId.Value);
        }

        
        var listvm = foods.Select(food => new FoodListVm
        {
            Id = food.Id,
            Name = food.Name,
            Price = food.Price,
            Description = food.Description,
            CategoryId= food.CategoryId,
            Category = food.Category.Name,
            Categories = categories

        }).ToList();
        var vm = new FoodFilterVm
        {
            Foods = listvm,
            Categories = new SelectList(categories, "Value", "Text"),
            SelectedCategoryId = categoryId

        };
        return View(vm);
    }
   
    

   

    public IActionResult Create()
    {
        var categories = _categoryRepository.GetCategories();
        var vm = new FoodVm();
        vm.Categories = categories;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FoodVm vm)
    {
        if(!ModelState.IsValid)return View(vm);
        var dto = new FoodDto()
        {
            Name = vm.Name,
            Price = vm.Price,
            Description = vm.Description,
            CategoryId = vm.CategoryId
        };
        await _foodService.Create(dto);
        return RedirectToAction("Index");
    }

    

    public   IActionResult Edit(int id)
    {
        var dto = _FoodRepository.GetById(id);
        if(dto == null)return RedirectToAction("Index");
        var categories = _categoryRepository.GetCategories();

        dto.Categories = categories;
        return View(dto);
    }

    [HttpPost]
    public async Task <IActionResult> Edit( long id, FoodVm vm)
    {
        var dto = new FoodDto()
        {
            Id = id,
            Name = vm.Name,
            Price = vm.Price,

            Description = vm.Description,
            CategoryId = vm.CategoryId
        };
        
       await _foodService.Edit(dto);
        return RedirectToAction("Index");

    }

    

    public IActionResult Delete(long id)
    {
        var dto = _FoodRepository.GetById(id);
        if (dto == null) return RedirectToAction("Index");
        _foodService.Delete(id);
        return RedirectToAction("Index");
    }

}

    