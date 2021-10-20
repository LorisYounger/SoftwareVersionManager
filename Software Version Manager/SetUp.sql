#本sql是创建WWCMS所需的数据库
#创建完成后别忘了在web.config改设置

#创建设置表
CREATE TABLE `setting` ( `selector` VARCHAR(64) NOT NULL COMMENT '设置项目' , `property` TEXT NOT NULL COMMENT '设置内容' ) ENGINE = InnoDB COMMENT = '设置表';ALTER TABLE `setting` ADD PRIMARY KEY(`selector`);

#---注意:如果是和其他WWCMS或其他兼容使用相同的用户表 无需创建用户表

#创建用户表
CREATE TABLE `users` ( `Uid` INT NOT NULL AUTO_INCREMENT COMMENT '用户id' , `name` VARCHAR(30) NOT NULL COMMENT '用户名' , `email` VARCHAR(40) NOT NULL COMMENT '电子邮件' , `password` VARCHAR(32) NOT NULL COMMENT '密码md5s' , `isroot` BOOLEAN NOT NULL DEFAULT FALSE COMMENT '是超级管理员' , `money` INT NOT NULL DEFAULT '0' COMMENT '金钱' , `exp` INT NOT NULL DEFAULT '0' COMMENT '经验值' , `lv` INT NULL DEFAULT '1' COMMENT '等级' , `headport` TINYTEXT NULL DEFAULT NULL COMMENT '头像url' , PRIMARY KEY (`Uid`), INDEX (`name`)) ENGINE = InnoDB COMMENT = '用户表';

#创建第一个默认用户
#账号 admin 密码 WWCMSpassword    Md5s=> bae82cfd819d8819e1011e944973af68
INSERT INTO `users` (`Uid`, `name`, `email`, `password`, `isroot`, `money`, `exp`, `lv`, `headport`) VALUES (NULL, 'admin', 'admin@exlb.org', 'bae82cfd819d8819e1011e944973af68', '1', '100', '0', '10', NULL);

#--------------------------------------------

#创建激活表
CREATE TABLE `activecode` ( `code` BIGINT NOT NULL COMMENT '激活码' , `software` VARCHAR(256) NOT NULL COMMENT '可以激活的软件名' , `verison` INT NOT NULL DEFAULT '-1' COMMENT '可激活版本' , `uid` INT NULL DEFAULT '-1' COMMENT '用户id' , `expiration` DATETIME NOT NULL COMMENT '失效日期' , `times` SMALLINT NOT NULL COMMENT '可以激活次数' , `illustration` TEXT NOT NULL COMMENT '描述' , `remarks` TEXT NOT NULL COMMENT '备注(给管理员)' , `activated` TEXT NOT NULL COMMENT '已激活电脑' , PRIMARY KEY (`code`), INDEX(`uid`)) ENGINE = InnoDB COMMENT = '激活码表';

#创建版本表
CREATE TABLE `verisonmanager` ( `Vid` INT NOT NULL AUTO_INCREMENT COMMENT '用于键控' , `software` VARCHAR(64) NOT NULL COMMENT '软件名称' , `ver` INT NOT NULL COMMENT '软件版本' , `verison` VARCHAR(32) NOT NULL COMMENT '软件版本(文本)' , `publishdate` DATETIME NOT NULL COMMENT '发布时间' , `importances` TINYINT NOT NULL COMMENT '重要性' , `times` INT NOT NULL COMMENT '被使用次数' , `illustration` TEXT NOT NULL COMMENT '更新内容' , `remarks` TEXT NOT NULL COMMENT '备注(给管理员)' , PRIMARY KEY (`Vid`), INDEX (`software`)) ENGINE = InnoDB;

