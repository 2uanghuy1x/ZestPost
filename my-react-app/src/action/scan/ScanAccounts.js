import React, { useState, useEffect, useRef } from 'react';
import { csharpApi } from '../../api';
import './ScanAccounts.css';

function ScanAccounts() {
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [accounts, setAccounts] = useState([]);
    const [filteredAccounts, setFilteredAccounts] = useState([]);
    const [selectedAccountIds, setSelectedAccountIds] = useState({}); // {accountId: true/false}
    const [scanType, setScanType] = useState('page'); // 'page' or 'group'
    const [scanGroupsInPage, setScanGroupsInPage] = useState(false);
    const [scannedPages, setScannedPages] = useState([]);
    const [scannedGroups, setScannedGroups] = useState([]);
    const [isScanning, setIsScanning] = useState(false);
    const [scanResult, setScanResult] = useState(null); // To show raw result or status
    const [searchTerm, setSearchTerm] = useState('');

    // Using a ref to keep track of the scan process for the stop button
    const scanProcessRef = useRef(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'scanResult') {
                // This is a simplified handler. In a real app, you'd parse the result
                // and populate scannedPages/scannedGroups more granularly.
                console.log("Scan Result:", message.payload);
                setScanResult(message.payload); // Display raw result for now

                // For demonstration, let's assume the scanResult might contain new pages/groups
                // In a real scenario, the backend would send dedicated messages like 'newPagesData' or 'newGroupsData'
                // For now, we'll just re-fetch all pages and groups to show a refreshed state
                csharpApi.getPageAccounts(); // Re-fetch all pages
                csharpApi.getGroupAccounts(); // Re-fetch all groups

                setIsScanning(false);
            }
            // We need to handle specific data updates for pages and groups when they are fetched
            if (message.action === 'pageAccountsData') {
                setScannedPages(message.payload);
            }
            if (message.action === 'groupAccountsData') {
                setScannedGroups(message.payload);
            }
            if (message.action === 'actionSuccess') {
                 // Re-fetch data after a deletion or other successful action
                csharpApi.getPageAccounts();
                csharpApi.getGroupAccounts();
                csharpApi.getAccounts();
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getAccounts();
        csharpApi.getCategories();
        csharpApi.getPageAccounts(); // Initial load for all pages
        csharpApi.getGroupAccounts(); // Initial load for all groups

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
            // Clear any ongoing scan if component unmounts
            if (scanProcessRef.current) {
                clearTimeout(scanProcessRef.current);
            }
        };
    }, []);

    useEffect(() => {
        let currentFilteredAccounts = accounts;

        if (selectedCategory) {
            currentFilteredAccounts = currentFilteredAccounts.filter(
                (account) => account.categoryId === parseInt(selectedCategory)
            );
        }

        if (searchTerm) {
            currentFilteredAccounts = currentFilteredAccounts.filter(
                (account) =>
                    account.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                    account.uid.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }

        setFilteredAccounts(currentFilteredAccounts);
    }, [accounts, selectedCategory, searchTerm]);

    const handleAccountSelection = (accountId, isSelected) => {
        setSelectedAccountIds(prev => ({
            ...prev,
            [accountId]: isSelected
        }));
    };

    const handleSelectAllAccounts = (isSelected) => {
        const newSelectedAccounts = {};
        filteredAccounts.forEach(account => {
            newSelectedAccounts[account.id] = isSelected;
        });
        setSelectedAccountIds(newSelectedAccounts);
    };

    const handleStartScan = () => {
        const accountsToScan = Object.keys(selectedAccountIds).filter(id => selectedAccountIds[id]).map(id => accounts.find(acc => acc.id === parseInt(id)));

        if (accountsToScan.length === 0) {
            alert("Vui lòng chọn ít nhất một tài khoản để bắt đầu quét.");
            return;
        }

        setIsScanning(true);
        setScanResult(null);
        setScannedPages([]); // Clear previous results
        setScannedGroups([]); // Clear previous results

        // Simulate scan process and call C# API for each selected account
        let index = 0;
        const performScanStep = () => {
            if (index < accountsToScan.length && isScanning) {
                const account = accountsToScan[index];
                console.log(`Scanning account: ${account.name} (ID: ${account.id}) for ${scanType}`);

                if (scanType === 'page') {
                    // Assuming backend can handle `scanGroups` property
                    csharpApi.scanPageAccount({ accountId: account.id, scanGroups: scanGroupsInPage });
                } else if (scanType === 'group') {
                    csharpApi.scanGroupAccount({ accountId: account.id });
                }

                index++;
                // Schedule next scan step with a delay (simulating async operation)
                scanProcessRef.current = setTimeout(performScanStep, 1000); // 1 second delay per account
            } else {
                setIsScanning(false);
                console.log("Scan process finished or stopped.");
            }
        };

        performScanStep();
    };

    const handleStopScan = () => {
        setIsScanning(false);
        if (scanProcessRef.current) {
            clearTimeout(scanProcessRef.current);
            scanProcessRef.current = null;
            // In a real application, you would also send a message to C# to stop the ongoing native process.
            // csharpApi.stopScan(); // Assuming such a method exists
            alert("Quá trình quét đã được dừng.");
        }
    };

    const handleDeletePage = (pageAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản trang ${pageAccount.name} này không?`)) {
            csharpApi.deletePageAccount(pageAccount);
        }
    };

    const handleDeleteGroup = (groupAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản nhóm ${groupAccount.name} này không?`)) {
            csharpApi.deleteGroupAccount(groupAccount);
        }
    };

    const getCategoryName = (categoryId) => {
        const category = categories.find(c => c.id === categoryId);
        return category ? category.name : 'N/A';
    };

    const accountCategories = categories.filter(c => c.type === 'account' || !c.type); 
    const allFilteredAccountsSelected = filteredAccounts.length > 0 && filteredAccounts.every(account => selectedAccountIds[account.id]);
    const isStartButtonDisabled = isScanning || Object.keys(selectedAccountIds).filter(id => selectedAccountIds[id]).length === 0;

    return (
        <div className="scan-accounts-container">
            <div className="main-scan-section">
                <div className="card account-selection-card">
                    <div className="card-header">
                        <h3>Chọn Tài khoản để Quét</h3>
                    </div>
                    <div className="controls-grid">
                        <div className="form-group">
                            <label htmlFor="categoryFilter">Lọc theo Danh mục:</label>
                            <select
                                id="categoryFilter"
                                value={selectedCategory}
                                onChange={(e) => setSelectedCategory(e.target.value)}
                            >
                                <option value="">Tất cả danh mục</option>
                                {accountCategories.map(cat => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                        </div>
                        <div className="form-group">
                            <label htmlFor="searchTerm">Tìm kiếm Tài khoản:</label>
                            <input
                                type="text"
                                id="searchTerm"
                                placeholder="Tìm theo tên hoặc UID"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                    </div>

                    <div className="table-container account-table-container">
                        <table className="crud-table">
                            <thead>
                                <tr>
                                    <th style={{ width: '30px' }}>
                                        <input
                                            type="checkbox"
                                            onChange={(e) => handleSelectAllAccounts(e.target.checked)}
                                            checked={allFilteredAccountsSelected}
                                        />
                                    </th>
                                    <th>Tên</th>
                                    <th>UID</th>
                                    <th>Danh mục</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredAccounts.length > 0 ? (
                                    filteredAccounts.map((account) => (
                                        <tr key={account.id}>
                                            <td>
                                                <input
                                                    type="checkbox"
                                                    checked={!!selectedAccountIds[account.id]}
                                                    onChange={(e) => handleAccountSelection(account.id, e.target.checked)}
                                                />
                                            </td>
                                            <td>{account.name}</td>
                                            <td>{account.uid}</td>
                                            <td>{getCategoryName(account.categoryId)}</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="4">Không có tài khoản nào phù hợp.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div className="scan-options-section">
                <div className="card scan-control-card">
                    <div className="card-header">
                        <h3>Tùy chọn Quét</h3>
                    </div>
                    <div className="scan-options-content">
                        <div className="form-group">
                            <label htmlFor="scanType">Loại Quét:</label>
                            <select
                                id="scanType"
                                value={scanType}
                                onChange={(e) => setScanType(e.target.value)}
                                disabled={isScanning}
                            >
                                <option value="page">Quét Page</option>
                                <option value="group">Quét Group</option>
                            </select>
                        </div>

                        {scanType === 'page' && (
                            <div className="form-group checkbox-group">
                                <input
                                    type="checkbox"
                                    id="scanGroupsInPage"
                                    checked={scanGroupsInPage}
                                    onChange={(e) => setScanGroupsInPage(e.target.checked)}
                                    disabled={isScanning}
                                />
                                <label htmlFor="scanGroupsInPage">Quét nhóm trong Page</label>
                            </div>
                        )}
                        
                        <div className="scan-buttons">
                            <button 
                                className="action-btn start-scan" 
                                onClick={handleStartScan} 
                                disabled={isStartButtonDisabled}
                            >
                                {isScanning ? 'Đang quét...' : 'Bắt đầu Quét'}
                            </button>
                            <button 
                                className="action-btn stop-scan" 
                                onClick={handleStopScan} 
                                disabled={!isScanning}
                            >
                                Dừng lại
                            </button>
                        </div>
                    </div>
                </div>

                {scanResult && (
                    <div className="card scan-result-card">
                        <div className="card-header">
                            <h3>Kết quả Quét</h3>
                        </div>
                        <div className="scan-result-content">
                            <pre>{JSON.stringify(scanResult, null, 2)}</pre>
                        </div>
                    </div>
                )}
            </div>

            <div className="scan-results-tables">
                {(scanType === 'page' || (scanType === 'page' && scanGroupsInPage)) && (
                    <div className="card page-table-card">
                        <div className="card-header">
                            <h3>Danh sách Trang</h3>
                        </div>
                        <div className="table-container">
                            <table className="crud-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Tên Trang</th>
                                        <th>Page ID</th>
                                        <th className="actions-column">Hành động</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {scannedPages.length > 0 ? (
                                        scannedPages.map((page) => (
                                            <tr key={page.id}>
                                                <td>{page.id}</td>
                                                <td>{page.name}</td>
                                                <td>{page.pageId}</td>
                                                <td className="actions-column">
                                                    <button className="action-btn delete" onClick={() => handleDeletePage(page)}>Xóa</button>
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan="4">Không có trang nào được tìm thấy.</td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}

                {((scanType === 'page' && scanGroupsInPage) || scanType === 'group') && (
                    <div className="card group-table-card">
                        <div className="card-header">
                            <h3>Danh sách Nhóm</h3>
                        </div>
                        <div className="table-container">
                            <table className="crud-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Tên Nhóm</th>
                                        <th>Group ID</th>
                                        <th className="actions-column">Hành động</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {scannedGroups.length > 0 ? (
                                        scannedGroups.map((group) => (
                                            <tr key={group.id}>
                                                <td>{group.id}</td>
                                                <td>{group.name}</td>
                                                <td>{group.groupId}</td>
                                                <td className="actions-column">
                                                    <button className="action-btn delete" onClick={() => handleDeleteGroup(group)}>Xóa</button>
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan="4">Không có nhóm nào được tìm thấy.</td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default ScanAccounts;
