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
    addHistoryAccount: (historyAccount) => post({ action: 'mstAddHistoryAccount', payload: historyAccount }),
    updateHistoryAccount: (historyAccount) => post({ action: 'mstUpdateHistoryAccount', payload: historyAccount }),
    deleteHistoryAccount: (historyAccount) => post({ action: 'mstDeleteHistoryAccount', payload: historyAccount }),

    // Page Account Actions
    getPageAccounts: () => post({ action: 'mstGetPageAccounts' }),
    addPageAccount: (pageAccount) => post({ action: 'mstAddPageAccount', payload: pageAccount }),
    updatePageAccount: (pageAccount) => post({ action: 'mstUpdatePageAccount', payload: pageAccount }),
    deletePageAccount: (pageAccount) => post({ action: 'mstDeletePageAccount', payload: pageAccount }),

    // Group Account Actions
    getGroupAccounts: () => post({ action: 'mstGetGroupAccounts' }),
    addGroupAccount: (groupAccount) => post({ action: 'mstAddGroupAccount', payload: groupAccount }),
    updateGroupAccount: (groupAccount) => post({ action: 'mstUpdateGroupAccount', payload: groupAccount }),
    deleteGroupAccount: (groupAccount) => post({ action: 'mstDeleteGroupAccount', payload: groupAccount }),
    
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
