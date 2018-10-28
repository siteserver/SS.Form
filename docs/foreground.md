# 模板 STL 标签调用

后台录入完毕表单信息后，需要在模板中添加表单信息相关标签，将表单信息显示在页面中。

## 表单信息内容列表标签

显示表单信息内容列表需要使用stl:contents或者stl:pageContents标签：

```
<stl:contents channelIndex="加入我们">
    <stl:a>
        职位：<stl:content type="Title"></stl:content>
        部门：<stl:content type="Department"></stl:content>
    </stl:a>
</stl:contents>
```

此标签通常放在首页模板或者栏目模板中，用以显示表单信息列表。

注：以上标签只是示例，实际使用需要考虑样式以及属性。

## 表单信息内容标签

通常在内容模板中显示详细的表单信息，表单信息使用stl:content标签，通过type属性获取具体的表单数据项。

```
职位：<stl:content type="Title"></stl:content>
所属部门：<stl:content type="Department"></stl:content>
工作地点：<stl:content type="Location"></stl:content>
表单人数：<stl:content type="NumberOfPeople"></stl:content>
工作职责：<stl:content type="Responsibility"></stl:content>
工作要求：<stl:content type="Requirement"></stl:content>

```

## 表单内容字段

下表显示详细的表单内容字段：

| 属性           | 类型         | 说明                               |
| -------------- | ------------ | ---------------------------------- |
| Title          | 标题（职位） | 表单内容标题，通常为表单的职位信息 |
| Department     | 所属部门     | 如技术部、客户服务部等             |
| NumberOfPeople | 表单人数     | 填写数字或任意文字，默认为不限     |
| Responsibility | 工作职责     | 详细描述此岗位的工作职责           |
| Requirement    | 工作要求     | 详细描述应聘此岗位的要求           |

其他属性包括 Id、AddDate等系统字段，可以根据需要使用。