var $getParameter = function (name) {
  var result = location.search.match(
    new RegExp('[?&]' + name + '=([^&]+)', 'i')
  );
  if (!result || result.length < 1) {
    return window.$config ? $config[name] : '';
  }
  return decodeURIComponent(result[1]);
};

var $api = axios.create({
  withCredentials: true
});

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: "#form_list",
  data: {
    apiUrl: $getParameter('apiUrl'),
    siteId: $getParameter('siteId'),
    formId: $getParameter('formId'),
    pageType: 'loading',
    fieldInfoList: null,
    allAttributeNames: [],
    listAttributeNames: [],
    isReply: false,
    page: 1,
    items: [],
    count: null,
    pages: null,
    pageOptions: null,
  },
  methods: {
    getAttributeText: function (attributeName) {
      if (attributeName === 'AddDate') {
        return '添加时间';
      } else if (attributeName === 'IsReplied') {
        return '是否回复';
      } else if (attributeName === 'ReplyDate') {
        return '回复时间';
      } else if (attributeName === 'ReplyContent') {
        return '回复内容';
      }

      return attributeName;
    },

    getAttributeValue: function (item, attributeName) {
      if (attributeName === 'IsReplied') {
        return item.isReplied ? '<strong class="text-primary">已回复</strong>' : '<strong class="text-danger">未回复</strong>';
      } else if (attributeName === 'ReplyDate') {
        return item.isReplied ? item.replyDate : '';
      } else if (attributeName === 'ReplyContent') {
        return item.isReplied ? item.replyContent : '';
      }

      return item[_.camelCase(attributeName)];
    },

    loadFirstPage: function () {
      if (this.page === 1) return;
      this.loadPage(1);
    },

    loadPrevPage: function () {
      if (this.page - 1 <= 0) return;
      this.loadPage(this.page - 1);
    },

    loadNextPage: function () {
      if (this.page + 1 > this.pages) return;
      this.loadPage(this.page + 1);
    },

    loadLastPage: function () {
      if (this.page + 1 > this.pages) return;
      this.loadPage(this.pages);
    },

    onPageSelect: function (option) {
      this.loadPage(option);
    },

    loadPage: function (page) {
      var $this = this;

      this.pageType = 'loading';
      $api.get(this.apiUrl + '/ss.form/' + this.siteId + '/' + this.formId, {
        page: page
      }).then(function (res) {
        $this.fieldInfoList = res.data.fieldInfoList;
        $this.allAttributeNames = res.data.allAttributeNames;
        $this.listAttributeNames = res.data.listAttributeNames;
        $this.isReply = res.data.isReply;
        $this.items = res.data.value;
        $this.count = res.data.count;
        $this.pages = res.data.pages;
        $this.page = res.data.page;
        $this.pageOptions = [];
        for (var i = 1; i <= $this.pages; i++) {
          $this.pageOptions.push(i);
        }

        $this.pageType = 'list';
        document.documentElement.scrollTop = document.body.scrollTop = 0;
      });
    }
  },
  created: function () {
    this.loadPage(1);
  }
});