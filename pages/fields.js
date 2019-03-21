var $url = '/pages/fields';
var $urlExport = '/pages/fields/actions/export';
var $urlImport = '/pages/fields/actions/import';

var $apiUrl = utils.getQueryString('apiUrl');
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $returnUrl = utils.getQueryString('returnUrl');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  items: null,
  uploadUrl: null,
  files: []
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.uploadUrl = $api.defaults.baseURL + $urlImport + '?adminToken=' + res.adminToken + '&siteId=' + $siteId + '&formId=' + $formId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (fieldId) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        fieldId: fieldId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  inputFile(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.import.active) {
        this.$refs.import.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      swal2({
        title: '字段导入成功',
        type: 'success',
        confirmButtonText: '确 定',
        confirmButtonClass: 'btn btn-primary',
      }).then(function (result) {
        if (result.value) {
          location.reload(true);
        }
      });

    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(zip)$/i.test(newFile.name)) {
        swal2({
          title: '上传格式错误！',
          text: '请上传zip压缩包',
          type: 'error',
          confirmButtonText: '确 定',
          confirmButtonClass: 'btn btn-primary',
        });
        return prevent()
      }
    }
  },

  btnEditClick: function (fieldId) {
    utils.openLayer({
      title: '编辑字段',
      url: 'fieldsLayerStyle.html?siteId=' + $siteId + '&formId=' + $formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnValidateClick: function (fieldId) {
    utils.openLayer({
      title: '设置验证规则',
      url: 'fieldsLayerValidate.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: 'fieldsLayerStyle.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnExportClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlExport, {
      siteId: $siteId,
      channelId: $channelId,
      contentId: $contentId,
      formId: $formId
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnDeleteClick: function (title, fieldId) {
    var $this = this;

    utils.alertDelete({
      title: '删除字段',
      text: '此操作将删除字段 ' + title + '，确定吗？',
      callback: function () {
        $this.delete(fieldId);
      }
    });
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  }
};

new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.getList();
  }
});
