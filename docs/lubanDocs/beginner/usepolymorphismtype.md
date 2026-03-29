# 使用多态类型
> Source: https://www.datable.cn/docs/beginner/usepolymorphismtype

## 使用多态类型

并不是所有数据结构都是规范一致的，在复杂的GamePlay玩法中，经常出现Buff效果有非常多种类型，每种类型的字段都不一样。 如果简单地取它们的并集，即取一个大结构包含所有类型的字段，配置将非常臃肿和复杂。多态类型完美解决了此问题。

## 定义

我们以Shape为例，Shape有很多子类，如圆、三角形、矩形。定义如下：

![item](/assets/images/define_shape-95a8e5a8ee443a7826aa55570c2fc60f.jpg)

当一个类形有1个及以上子类时，称之为多态类型。每个子类型需要指定parent属性，如Circle的parent为Shape。

## 填写多态数据

多态结构如普通结构那样，支持流式与sep。填写多态数据时，第一个数据必须是多态类型，如Circle。填写多态类型时也支持别名，如`矩形`。

![item](/assets/images/use_shape1-8a284b753c692b370a01f29edf6b0b71.jpg)

支持列限定格式，此时需要用$type列来指定多态类型：

![item](/assets/images/use_shape2-1e1fd19702aa455d20677bc748b5474e.jpg)

支持多行数据：

![item](/assets/images/use_shape3-663cf51a766aa6fca89d2c05592dedc7.jpg)
