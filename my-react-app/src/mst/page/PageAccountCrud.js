import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './PageAccountCrud.css';

function PageAccountCrud() {
    const [pageAccounts, setPageAccounts] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentPageAccount, setCurrentPageAccount] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'pageAccountsData') {
                setPageAccounts(message.payload);
            }
            if (message.action === 'actionSuccess') {
                setIsModalOpen(false);
                setCurrentPageAccount(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getPageAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentPageAccount({
            name: '',
            pageId: '',
            accessToken: '',
            categoryId: null
        });
        setIsModalOpen(true);
    };

    const handleEdit = (pageAccount) => {
        setCurrentPageAccount(pageAccount);
        setIsModalOpen(true);
    };

    const handleDelete = (pageAccount) => {
        if (window.confirm(`Are you sure you want to delete page account ${pageAccount.name}?`)) {
            csharpApi.deletePageAccount(pageAccount);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        const pageAccountToSave = {
            ...currentPageAccount,
            categoryId: parseInt(currentPageAccount.categoryId, 10) || null
        };
        if (pageAccountToSave.id) {
            csharpApi.updatePageAccount(pageAccountToSave);
        } else {
            csharpApi.addPageAccount(pageAccountToSave);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentPageAccount({ ...currentPageAccount, [name]: value });
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản Trang</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên Trang</th>
                                <th>Page ID</th>
                                <th>Access Token</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {pageAccounts.map((pageAccount) => (
                                <tr key={pageAccount.id}>
                                    <td>{pageAccount.name}</td>
                                    <td>{pageAccount.pageId}</td>
                                    <td>{pageAccount.accessToken ? '********' : ''}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(pageAccount)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(pageAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>{currentPageAccount && currentPageAccount.id ? 'Sửa tài khoản trang' : 'Thêm tài khoản trang'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <div className="form-group">
                                <label>Tên Trang:</label>
                                <input name="name" value={currentPageAccount?.name || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-group">
                                <label>Page ID:</label>
                                <input name="pageId" value={currentPageAccount?.pageId || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-group full-width">
                                <label>Access Token:</label>
                                <textarea name="accessToken" value={currentPageAccount?.accessToken || ''} onChange={handleInputChange} rows="4" required />
                            </div>
                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default PageAccountCrud;
