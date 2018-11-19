﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SellerBox.Models.Database
{
    public class Auction : BaseEntity
    {
        /// <summary>
        /// Наименование товара
        /// </summary>
        [DisplayName("Наименование аукциона")]
        public string Name { get; set; }
        
        /// <summary>
        /// Время публикации аукциона
        /// </summary>
        [DisplayName("Дата и время проведения аукциона")]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Время окончания аукциона
        /// </summary>
        [DisplayName("Дата и время окончание аукциона")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Описание аукциона
        /// </summary>
        [DisplayName("Описание аукциона")]
        public string Description { get; set; }

        /// <summary>
        /// Начальная цена
        /// </summary>
        [DisplayName("Начальная цена")]
        public decimal StartPrice { get; set; } = 100;

        /// <summary>
        /// Текущая цена
        /// </summary>
        public decimal CurrentPrice { get; set; }
        
        /// <summary>
        /// Шаг цены
        /// </summary>
        [DisplayName("Шаг цены")]
        public int PriceStep { get; set; } = 10;

        /// <summary>
        /// Максимальная цена
        /// </summary>
        [DisplayName("Максимальная цена")]
        public decimal MaxPrice { get; set; } = 1000;

        /// <summary>
        /// Максимальное количество комментариев
        /// </summary>
        [DisplayName("Максимальное количество комментариев")]
        public int MaxCommentsCount { get; set; } = 25;
        
        /// <summary>
        /// Активен ли аукцион
        /// </summary>
        public bool IsActive { get; set; }

        //TODO: подумать над отслеживанием публикации на стене
        //public bool Published { get; set; }
        
        /// <summary>
        /// id поста
        /// </summary>
        [ForeignKey(nameof(WallPosts))]
        public long IdPost { get; set; }
        
        /// <summary>
        /// id группы
        /// </summary>
        [ForeignKey(nameof(GroupAdmins))]
        public GroupAdmins IdCreator { get; set; }
    }
}