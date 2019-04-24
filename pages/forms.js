var $url = '/pages/forms';
var $urlActionsUp = '/pages/forms/actions/up';
var $urlActionsDown = '/pages/forms/actions/down';
var $urlExport = '/pages/forms/actions/export';
var $urlImport = '/pages/forms/actions/import';
$apiUrl = utils.getQueryString('apiUrl');
$siteId = utils.getQueryString('siteId');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  formInfoList: null,
  formInfo: null,
  uploadUrl: null,
  files: []
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get($url + '?siteId=' + $siteId).then(function (response) {
      var res = response.data;

      $this.formInfoList = res.value;
      $this.uploadUrl = $api.defaults.baseURL + $urlImport + '?adminToken=' + res.adminToken + '&siteId=' + $siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (formId) {
    var $this = this;

    utils.loading(true);
    $api.delete($url + '?siteId=' + $siteId + '&formId=' + formId).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单删除成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnViewClick: function (formId) {
    location.href = 'logs.html?siteId=' + $siteId + '&formId=' + formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent(location.href);
  },

  btnUpClick: function (formInfo) {
    var $this = this;

    utils.loading(true);
    $api.post($urlActionsUp, {
      siteId: $siteId,
      formId: formInfo.id
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnDownClick: function (formInfo) {
    var $this = this;

    utils.loading(true);
    $api.post($urlActionsDown, {
      siteId: $siteId,
      formId: formInfo.id
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (formInfo) {
    this.pageType = 'edit';
    this.formInfo = formInfo;
  },

  btnAddClick: function () {
    this.pageType = 'add';
    this.formInfo = {
      title: '',
      description: ''
    };
  },

  btnDeleteClick: function (formInfo) {
    var $this = this;

    utils.alertDelete({
      title: '删除表单',
      text: '此操作将删除表单，确定吗？',
      callback: function () {
        $this.delete(formInfo.id);
      }
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    utils.loading(true);
    if (this.formInfo.id) {
      $api.put($url, {
        siteId: $siteId,
        formId: this.formInfo.id,
        title: this.formInfo.title,
        description: this.formInfo.description
      }).then(function (response) {
        var res = response.data;

        swal2({
          toast: true,
          type: 'success',
          title: "表单修改成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.formInfoList = res.value;
        $this.pageType = 'list';
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        utils.loading(false);
      });
    } else {
      $api.post($url, {
        siteId: $siteId,
        title: this.formInfo.title,
        description: this.formInfo.description
      }).then(function (response) {
        var res = response.data;

        swal2({
          toast: true,
          type: 'success',
          title: "表单添加成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.formInfoList = res.value;
        $this.pageType = 'list';
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        utils.loading(false);
      });
    }
  },

  inputFile(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.import.active) {
        this.$refs.import.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      swal2({
        title: '表单导入成功',
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

  btnExportClick: function (formInfo) {
    var $this = this;

    utils.loading(true);
    $api.post($urlExport, {
      siteId: $siteId,
      formId: formInfo.id
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
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