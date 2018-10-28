config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');

var $api = new utils.Api('/ss.form/forms');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  formInfoList: null,
  formInfo: null
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.formInfoList = res.value;
      $this.pageLoad = true;
    });
  },

  delete: function (formId) {
    var $this = this;

    utils.loading(true);
    $api.delete({
      siteId: config.siteId,
      formId: formId
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      alert({
        toast: true,
        type: 'success',
        title: "表单删除成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    });
  },

  btnViewClick: function (formId) {
    location.href = 'logs.html?siteId=' + config.siteId + '&formId=' + formId + '&apiUrl=' + encodeURIComponent(config.apiUrl) + '&returnUrl=' + encodeURIComponent(location.href);
  },

  btnUpClick: function (formInfo) {
    var $this = this;

    utils.loading(true);
    $api.post({
      siteId: config.siteId,
      formId: formInfo.id
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      alert({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    }, 'actions/up');
  },

  btnDownClick: function (formInfo) {
    var $this = this;

    utils.loading(true);
    $api.post({
      siteId: config.siteId,
      formId: formInfo.id
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      alert({
        toast: true,
        type: 'success',
        title: "表单排序成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.formInfoList = res.value;
    }, 'actions/down');
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
      $api.put({
        siteId: config.siteId,
        formId: this.formInfo.id,
        title: this.formInfo.title,
        description: this.formInfo.description
      }, function (err, res) {
        utils.loading(false);
        if (err || !res || !res.value) return;

        alert({
          toast: true,
          type: 'success',
          title: "表单修改成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.formInfoList = res.value;
        $this.pageType = 'list';
      });
    } else {
      $api.post({
        siteId: config.siteId,
        title: this.formInfo.title,
        description: this.formInfo.description
      }, function (err, res) {
        utils.loading(false);
        if (err || !res || !res.value) return;

        alert({
          toast: true,
          type: 'success',
          title: "表单添加成功",
          showConfirmButton: false,
          timer: 2000
        });
        $this.formInfoList = res.value;
        $this.pageType = 'list';
      });
    }
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});