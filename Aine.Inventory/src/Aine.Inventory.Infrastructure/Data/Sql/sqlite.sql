﻿CREATE TABLE "product_category" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_product_category" PRIMARY KEY AUTOINCREMENT,
    "name" TEXT NOT NULL,
    "description" TEXT NULL
);


CREATE TABLE "product_model" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_product_model" PRIMARY KEY AUTOINCREMENT,
    "name" TEXT NOT NULL,
    "description" TEXT NOT NULL
);


CREATE TABLE "product_subcategory" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_product_subcategory" PRIMARY KEY AUTOINCREMENT,
    "category_id" INTEGER NOT NULL,
    "name" TEXT NOT NULL,
    "description" TEXT NULL,
    CONSTRAINT "FK_product_subcategory_product_category_category_id" FOREIGN KEY ("category_id") REFERENCES "product_category" ("id") ON DELETE CASCADE
);


CREATE TABLE "product" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_product" PRIMARY KEY AUTOINCREMENT,
    "product_number" TEXT NOT NULL,
    "name" TEXT NOT NULL,
    "description" TEXT NULL,
    "sub_category_id" INTEGER NULL,
    "model_id" INTEGER NULL,
    "color" TEXT NULL,
    "size" TEXT NULL,
    "size_unit" TEXT NULL,
    "weight" REAL NULL,
    "weight_unit" TEXT NULL,
    "style" TEXT NULL,
    "reorder_point" INTEGER NULL,
    "standard_cost" REAL NULL,
    "list_price" REAL NULL,
    "is_active" INTEGER NOT NULL,
    "modified_date" TEXT NULL,
    CONSTRAINT "FK_product_product_model_model_id" FOREIGN KEY ("model_id") REFERENCES "product_model" ("id"),
    CONSTRAINT "FK_product_product_subcategory_sub_category_id" FOREIGN KEY ("sub_category_id") REFERENCES "product_subcategory" ("id")
);


CREATE TABLE "product_photo" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_product_photo" PRIMARY KEY AUTOINCREMENT,
    "product_id" INTEGER NOT NULL,    
    "thumbnail_photo_filename" TEXT NOT NULL,    
    "large_photo_filename" TEXT NOT NULL, 
    CONSTRAINT "FK_product_photo_product_product_id" FOREIGN KEY ("product_id") REFERENCES "product" ("id") ON DELETE CASCADE
);


CREATE INDEX "IX_product_model_id" ON "product" ("model_id");


CREATE UNIQUE INDEX "IX_product_product_number" ON "product" ("product_number");


CREATE INDEX "IX_product_sub_category_id" ON "product" ("sub_category_id");


CREATE UNIQUE INDEX "IX_product_category_name" ON "product_category" ("name");


CREATE UNIQUE INDEX "IX_product_model_name" ON "product_model" ("name");


CREATE UNIQUE INDEX "IX_product_photo_product_id" ON "product_photo" ("product_id");


CREATE INDEX "IX_product_subcategory_category_id" ON "product_subcategory" ("category_id");


CREATE UNIQUE INDEX "IX_product_subcategory_name" ON "product_subcategory" ("name");


