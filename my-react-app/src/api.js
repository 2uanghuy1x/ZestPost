const isWebView = window.chrome && window.chrome.webview;

const post = (message) => {
    if (isWebView) {
        window.chrome.webview.postMessage(message);
    } else {
        console.log('Skipping postMessage (not in WebView2):', message);
    }
};

export const csharpApi = {
    // Account Actions
    getAccounts: () => post({ action: 'mstGetAccounts' }),
    addAccount: (account) => post({ action: 'mstAddAccount', payload: account }),
    updateAccount: (account) => post({ action: 'mstUpdateAccount', payload: account }),
    deleteAccount: (account) => post({ action: 'mstDeleteAccount', payload: account }),
    startAccounts: (accountIds) => post({ action: 'mstStartAccounts', payload: accountIds }),
    stopAccounts: (accountIds) => post({ action: 'mstStopAccounts', payload: accountIds }),

    // Category Actions
    getCategories: () => post({ action: 'mstGetCategories' }),
    addCategory: (category) => post({ action: 'mstAddCategory', payload: category }),
    updateCategory: (category) => post({ action: 'mstUpdateCategory', payload: category }),
    deleteCategory: (category) => post({ action: 'mstDeleteCategory', payload: category }),

    // Article Actions
    getArticles: () => post({ action: 'mstGetArticles' }),
    addArticle: (article) => post({ action: 'mstAddArticle', payload: article }),
    updateArticle: (article) => post({ action: 'mstUpdateArticle', payload: article }),
    deleteArticle: (article) => post({ action: 'mstDeleteArticle', payload: article }),

    // History Account Actions
    getHistoryAccounts: () => post({ action: 'mstGetHistoryAccounts' }),
    deleteHistoryAccount: (historyAccount) => post({ action: 'mstDeleteHistoryAccount', payload: historyAccount }),

    // Page Account Actions
    getPageAccounts: () => post({ action: 'mstGetPageAccounts' }),
    getPagesByAccountId: (accountId) => post({ action: 'mstGetPagesByAccountId', payload: accountId }),
    deletePageAccount: (pageAccount) => post({ action: 'mstDeletePageAccount', payload: pageAccount }),
    scanPageAccount: (pageAccount) => post({ action: 'mstScanPageAccount', payload: pageAccount }),

    // Group Account Actions
    getGroupAccounts: () => post({ action: 'mstGetGroupAccounts' }),
    getGroupsByPageId: (pageId) => post({ action: 'mstGetGroupsByPageId', payload: pageId }),
    deleteGroupAccount: (groupAccount) => post({ action: 'mstDeleteGroupAccount', payload: groupAccount }),
    scanGroupAccount: (groupAccount) => post({ action: 'mstScanGroupAccount', payload: groupAccount }),
    
    // Post Article
    postArticle: (account) => post({ action: 'postArticle', payload: account }),

    // Event Listener Wrappers
    addEventListener: (event, handler) => {
        if (isWebView) {
            window.chrome.webview.addEventListener(event, handler);
        }
    },
    removeEventListener: (event, handler) => {
        if (isWebView) {
            window.chrome.webview.removeEventListener(event, handler);
        }
    }
};

// Export specific functions for ArticleManagement component
export const fetchArticles = async () => {
    return new Promise((resolve) => {
        const handler = (event) => {
            const message = event.data;
            if (message.action === 'articlesData') {
                resolve(message.payload);
                csharpApi.removeEventListener('message', handler);
            }
        };
        csharpApi.addEventListener('message', handler);
        csharpApi.getArticles();
    });
};

export const fetchCategories = async () => {
    return new Promise((resolve) => {
        const handler = (event) => {
            const message = event.data;
            if (message.action === 'categoriesData') {
                resolve(message.payload);
                csharpApi.removeEventListener('message', handler);
            }
        };
        csharpApi.addEventListener('message', handler);
        csharpApi.getCategories();
    });
};

export const fetchAccounts = async () => {
    return new Promise((resolve) => {
        const handler = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                resolve(message.payload);
                csharpApi.removeEventListener('message', handler);
            }
        };
        csharpApi.addEventListener('message', handler);
        csharpApi.getAccounts();
    });
};

